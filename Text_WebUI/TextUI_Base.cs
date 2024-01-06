using Discord.Commands;
using Discord.WebSocket;
using Discord_AI_Presence.Text_WebUI.DiscordStuff;
using Discord_AI_Presence.Text_WebUI.Instructions;
using Discord_AI_Presence.Text_WebUI.Presets;
using Discord_AI_Presence.Text_WebUI.ProfileScripts;
using Discord_AI_Presence.Text_WebUI.TextWebUI;
using Newtonsoft.Json;
using System.ComponentModel.Design;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Discord_AI_Presence.Text_WebUI
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class TextUI_Base : ITextUIBase
    {
        [JsonProperty]
        public List<TextUI_Servers> ServerData { get; init; } = [];
        public Dictionary<string, ProfileData> Cards { get; } = new(StringComparer.OrdinalIgnoreCase);
        public Queues NeuroNetCalls { get; init; } = new();
        private static TextUI_Base _instance = null;
        private static readonly object _lock = new();

        public TextUI_Base()
        {
            this.PopulateProfileDictionary();
        }

        public void PopulateServerData(List<SocketGuild> guilds)
        {
            // Client ready is run often whenever the client temporarily disconnects from the Discord API server and reconnects.
            // This is a sanity check to make sure that this server list isn't be populated more than once.
            if (ServerData.Count > 0)
                return;
            foreach (var guild in guilds)
            {
                ServerData.Add(new TextUI_Servers(guild.Id));
            }
        }

        /// <summary>
        /// Starts a normal chat where the AI scenario is that of chatting in an online community with random people.
        /// </summary>
        /// <param name="scc">The socket command context assigned whenever a message is received on Discord.</param>
        /// <param name="profile">The character name</param>
        public void StartChat(SocketCommandContext scc, string characterName)
        {
            var findServer = ServerData.FirstOrDefault(x => x.ServerID == scc.Guild.Id);
            if (findServer == null || findServer.ServerSettings.NoAIChannels.Exists(x => x == scc.Channel.Id))
                return;
            if (findServer.AiChatExists(scc.Channel.Id))
                return;
            findServer.StartChat(new MemoryManagement.Chats(scc.Channel.Id,
                findServer.ServerSettings.DefaultPreset,
                characterName,
                Scenario.GetScenario(Scenario.ScenarioPresets.Chatbot, characterName), scc.User.Id));
        }

        /// <summary>
        /// Starts a roleplay chat either with the default roleplay scenario or a custom scenario.
        /// </summary>
        /// <param name="scc">The socket command context assigned whenever a message is received on Discord.</param>
        /// <param name="characterName">The character name</param>
        /// <param name="customScenario">If no custom scenario specified, it will default to roleplay default.</param>
        public void StartChat(SocketCommandContext scc, string characterName, string customScenario = "")
        {
            var findServer = ServerData.FirstOrDefault(x => x.ServerID == scc.Guild.Id);
            if (findServer == null || findServer.ServerSettings.NoAIChannels.Exists(x => x == scc.Channel.Id))
                return;
            if (findServer.AiChatExists(scc.Channel.Id))
                return;
            findServer.StartChat(new MemoryManagement.Chats(scc.Channel.Id,
                findServer.ServerSettings.DefaultPreset,
                characterName,
                //Checks if a custom scenario is specified otherwise defaults to roleplay.
                string.IsNullOrEmpty(customScenario)
                ? Scenario.GetScenario(Scenario.ScenarioPresets.Roleplay, characterName)
                : Scenario.GetScenario(Scenario.ScenarioPresets.Default, characterName, customScenario), scc.User.Id));
        }

        [JsonConstructor]
        public TextUI_Base(List<TextUI_Servers> ServerData)
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
        public static ProfileData MatchName(string charFirstName)
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
            if (!filename.IsSafePath())
                filename = string.Empty;
            else
                filename = string.Concat("\\", filename, ".json");
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
                var jsonData = JsonConvert.DeserializeObject<ProfileData>(fileData);
                if (jsonData != null)
                    Cards.Add(jsonData.CharacterName, jsonData);
            }
        }

        /// <summary>
        /// If the character exists, it will grab its profile. Won't work if your bot is slash command only.
        /// </summary>
        /// <param name="msg">The message sent by the user</param>
        /// <returns>Null if character is not found, otherwise the character profile</returns>
        public ProfileData Check(string msg)
        {
            if (string.IsNullOrEmpty(msg))
                return null;
            var first = msg.Split(' ')[0]; // Makes sure Hey must appear at the beginning of the message to trigger it
            if (!first.Equals("hey", StringComparison.OrdinalIgnoreCase))
                return null;
            var redone = msg.ToLower().Replace("hey", string.Empty).Trim();
            if (!Cards.TryGetValue(redone, out ProfileData value))
                return null;
            return value;
        }

    }
}