using Discord.Commands;
using Discord.WebSocket;
using Discord_AI_Presence.Text_WebUI.Button_Related;
using Discord_AI_Presence.Text_WebUI.DiscordStuff;
using Discord_AI_Presence.Text_WebUI.DiscordStuff.API_Framework;
using Discord_AI_Presence.Text_WebUI.Instructions;
using Discord_AI_Presence.Text_WebUI.Presets;
using Discord_AI_Presence.Text_WebUI.ProfileScripts;
using Discord_AI_Presence.Text_WebUI.TextWebUI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.Design;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Discord_AI_Presence.Text_WebUI
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class TextUI_Base : ITextUIBase
    {
        /// <summary>
        /// The key is the guild ID.
        /// </summary>
        [JsonProperty]
        public Dictionary<ulong, TextUI_Servers> ServerData { get; init; } = [];
        public Dictionary<string, List<ProfileData>> Cards { get; init; } = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, Buttons> DuplicateHandling { get; init; } = [];
        public Queues NeuroNetCalls { get; init; } = new();
        public Webhooks Webhooks { get; init; } = new();
        private static TextUI_Base _instance = null;
        private static readonly object _lock = new();

        public TextUI_Base()
        {
            this.PopulateProfileDictionary();
        }

        public void PopulateServerData(List<SocketGuild> guilds)
        {
            foreach (var guild in guilds)
            {
                // Make sure there's no duplicates
                if (!ServerData.ContainsKey(guild.Id))
                    ServerData.Add(guild.Id, new TextUI_Servers(guild.Id));
            }
        }

        /// <summary>
        /// Starts a normal chat where the AI scenario is that of chatting in an online community with random people.
        /// </summary>
        /// <param name="scc">The socket command context assigned whenever a message is received on Discord.</param>
        public async Task StartChat(SocketCommandContext scc, string characterName, int index)
        {
            try
            {
                var server = ServerData[scc.Guild.Id];

                if (server.ServerSettings.NoAIChannels.Contains(scc.Channel.Id))
                    return;
                if (server.AiChatExists(scc.Channel.Id))
                {
                    var chat = server.FindChat(scc.Channel.Id);
                    chat.AddMessage(scc.User.GlobalName ?? scc.User.Username, scc.Message.Content, scc.User.Id, scc.Message.Id);
                    return;
                }
                if (!Cards.TryGetValue(characterName, out var profile))
                    return;
                var newCharacterJObj = JsonConvert.SerializeObject(profile[index]);
                var newCharacter = JsonConvert.DeserializeObject<ProfileData>(newCharacterJObj);
                var chatParameters = new ChatParameters(newCharacter.NickOrName(), scc.Message.Content);
                var chatSettings = new MemoryManagement.Chats(
                    scc.Channel.Id,
                    profile[index],
                    server.ServerSettings.DefaultPreset,
                    scc.User.GlobalName ?? scc.User.Username,
                    Scenario.ScenarioPresets.Chatbot,
                    scc.User.Id,
                    index
                );

                server.StartChat(chatSettings);
                await Webhooks.SendWebhookMessage(scc.Guild, profile[index], server.ServerSettings, profile[index].CharacterIntroduction, scc.Channel.Id);
            }
            catch (Exception m)
            {
                DebugThings.DebugExtensions.Log(m);
            }
        }

        /// <summary>
        /// Starts a chat with the specified character. This shouldn't be used in the MessageReceived delegate as it doesn't add messages to a pre-existing chat.
        /// </summary>
        /// <param name="guildId">The Discord guild ID</param>
        /// <param name="channelId">The Discord channel ID</param>
        /// <param name="userId">The Discord user ID who started the chat</param>
        /// <param name="profile">The character profile</param>
        /// <param name="index">Index is necessary if there are more than one character with the same name. Can leave at 0 in most cases it's handled automatically.</param>
        /// <returns></returns> 
        public async Task StartChat(ulong guildId, ulong channelId, ulong userId, ProfileData profile, int index, Scenario.ScenarioPresets convoType)
        {
            var server = ServerData[guildId];

            if (server.ServerSettings.NoAIChannels.Contains(channelId))
                return;
            var chatSettings = new MemoryManagement.Chats(
                channelId,
                profile,
                server.ServerSettings.DefaultPreset,
                profile.NickOrName(),
                convoType,
                userId,
                index
            );

            server.StartChat(chatSettings);

            await Webhooks.SendWebhookMessage(Client.GetInstance().FindGuild(guildId), profile, server.ServerSettings, profile.CharacterIntroduction, channelId);
        }


        /// <summary>
        /// Starts a roleplay chat with a custom scenario.
        /// </summary>
        /// <param name="scc">The socket command context assigned whenever a message is received on Discord.</param>
        /// <param name="characterName">The character name</param>
        /// <param name="index">Index is necessary if there are more than one character with the same name. Can leave at 0 in most cases it's handled automatically.</param>
        /// <param name="customScenario">If no custom scenario specified, it will default to roleplay default.</param>
        public async Task StartChat(SocketCommandContext scc, string characterName, int index = 0, string customScenario = "")
        {
            var server = ServerData[scc.Guild.Id];

            if (server == null || server.ServerSettings.NoAIChannels.Contains(scc.Channel.Id) || server.AiChatExists(scc.Channel.Id))
                return;

            if (!Cards.TryGetValue(characterName, out var profile))
                return;

            var preset = server.ServerSettings.DefaultPreset;
            var scenario = string.IsNullOrEmpty(customScenario)
                ? Scenario.ScenarioPresets.Roleplay
                : Scenario.ScenarioPresets.Default;

            server.StartChat(new MemoryManagement.Chats(scc.Channel.Id, profile[index], preset, characterName, scenario, scc.User.Id, index, customScenario));

            await Webhooks.SendWebhookMessage(scc.Guild, profile[index], server.ServerSettings, profile[index].CharacterIntroduction, scc.Channel.Id);
        }

        [JsonConstructor]
        public TextUI_Base(Dictionary<ulong, TextUI_Servers> ServerData)
        {
            this.ServerData = ServerData;
            _instance = this;
        }

        /// <summary>
        /// Singleton instance to use whenever data needs to be read for the Text AI
        /// </summary>
        /// <returns>The class instance</returns>
        public static TextUI_Base GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new TextUI_Base();
                }
            }
            return _instance;
        }

        /// <summary>
        /// Searches the dictionary of character profiles and returns the result.
        /// Dictionary is case insensitive by default.
        /// </summary>
        /// <param name="charFirstName">Search only by the character's first name</param>
        /// <returns>Returns null if nothing found.</returns>
        public static List<ProfileData> MatchName(string charFirstName)
        {
            if (GetInstance().Cards.TryGetValue(charFirstName, out var charData))
            {
                return charData;
            }

            return null;
        }

        /// <summary>
        /// Location of character profile cards using the datapath
        /// </summary>
        /// <param name="filename">Do not add any special characters or the file extension</param>
        /// <returns></returns>
        public string ProfileLocation(string filename = "")
        {
            if (!string.IsNullOrEmpty(filename))
            {
                if (!filename.IsSafePath())
                    filename = string.Empty;
                else
                    filename = string.Concat("\\", filename, ".json");
            }
            return $@"{Environment.CurrentDirectory}\TextUI_Files\AiProfileCards{filename}";
        }

        /// <summary>
        /// Character files stored in a folder using json format are grabbed and deserialized into the ProfileData data class.
        /// </summary>
        public void PopulateProfileDictionary()
        {
            var di = new DirectoryInfo(ProfileLocation());
            var fi = di.GetFiles();
            Cards.Clear();
            foreach (var files in fi)
            {
                var fileData = File.ReadAllText(files.FullName);
                /* Some cards seem to cause issues because their alternative greetings are replaced with an integer.
                 * This integer is useless for here as we need an array of greetings and do not have access to the database that integer is for. 
                 * This finds cards like this and simply reassigns it as a JArray to get past the null exception.
                 */
                dynamic jsonObj = JsonConvert.DeserializeObject(fileData);
                ProfileData jsonData = null;

                if (jsonObj.alternate_greetings != null)
                {
                    Type type = jsonObj.alternate_greetings.GetType();
                    if (type != typeof(JArray))
                        jsonObj.alternate_greetings = new JArray();
                }
                if (jsonObj.char_name != null)
                {
                    jsonObj.name = jsonObj.char_name;
                }
                if (jsonObj.world_scenario != null)
                {
                    jsonObj.scenario = jsonObj.world_scenario;
                }
                if (jsonObj.data != null) // For V2 cards to make sure we get the right JArray of data.
                    jsonData = JsonConvert.DeserializeObject<ProfileData>(jsonObj.data.ToString());
                else // If not a V2 card
                    jsonData = JsonConvert.DeserializeObject<ProfileData>(jsonObj.ToString());
                if (!Cards.ContainsKey(jsonData.NickOrName()))
                    Cards.Add(jsonData.Name, new() { jsonData });
                else // If a character with the same name already exists, add it to the profile list of this key instead.
                    Cards[jsonData.Name].Add(jsonData);
            }
        }

        /// <summary>
        /// If the character exists, it will grab its profile. Won't work if your bot is slash command only.
        /// </summary>
        /// <param name="msg">The message sent by the user</param>
        /// <returns>Null if character is not found, otherwise the character profile</returns>
        public List<ProfileData> Check(string msg, bool chatExists)
        {
            if (string.IsNullOrEmpty(msg))
                return null;
            var first = msg.Split(' ')[0]; // Makes sure Hey must appear at the beginning of the message to trigger it
            if (!first.Equals("hey", StringComparison.OrdinalIgnoreCase) && !chatExists)
                return null;
            var redone = msg.Remove(0, 3).Trim();
            if (!Cards.TryGetValue(redone, out List<ProfileData> value))
            {
                string keyToCheck = Cards.Keys.FirstOrDefault(x => x.Split(' ')[0].Equals(redone, StringComparison.OrdinalIgnoreCase));
                if (string.IsNullOrEmpty(keyToCheck))
                    return null;
                else
                    return Cards[keyToCheck];
            }
            return value;
        }

        /// <summary>
        /// Checks if the character's name is mentioned. If it is, they will reply right away.
        /// </summary>
        /// <param name="msg">the message sent in Discord</param>
        /// <param name="charName">The character name</param>
        /// <returns>True if the name is detected</returns>
        public bool CharShouldReply(string msg, string charName)
        {
            if (string.IsNullOrEmpty(msg))
                return false;
            return msg.Contains(charName, StringComparison.OrdinalIgnoreCase);
        }
    }
}