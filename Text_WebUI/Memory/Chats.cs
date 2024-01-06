using Discord_AI_Presence.Text_WebUI.DiscordStuff.API_Framework;
using Discord_AI_Presence.Text_WebUI.Instructions;
using Discord_AI_Presence.Text_WebUI.Presets;
using Discord_AI_Presence.Text_WebUI.ProfileScripts;
using Newtonsoft.Json;
using System.Text;

namespace Discord_AI_Presence.Text_WebUI.MemoryManagement
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    /// <summary>
    /// Thread IDs are not the same as channel IDs. Check whether it's a thread or not and assign the right ID.
    /// </summary>        
    public class Chats : ChatHistoryManager
    {
        public const int CHARACTER_ID = 000000;
        public TextUI_Presets Presets { get; init; }
        public ProfileData CharacterProfile { get; init; }
        /// <summary>
        /// Will not be serialized. This reduces client calls to get usernames.
        /// </summary>
        public string Username { get; init; }
        [JsonProperty]
        public ulong ChatStarterUserID { get; init; }
        [JsonProperty]
        public ulong ChannelID { get; init; }
        /// <summary>
        /// Instead of serializing entire character profiles, this will be used to call the character upon reload.
        /// </summary>
        [JsonProperty]
        public string CharacterName { get; init; }
        /// <summary>
        /// Instead of serializing entire preset files, this will be used to simply call the preset.
        /// </summary>
        [JsonProperty]
        public TextUI_Presets.PresetEnum PresetName { get; init; }
        [JsonProperty]
        public string ScenarioInfo { get; init; }

        [JsonConstructor]
        public Chats(ulong ChannelID, TextUI_Presets.PresetEnum PresetName, string CharacterName, string ScenarioInfo, ulong ChatStarterUserID)
        {
            this.ChannelID = ChannelID;
            Presets = new TextUI_Presets();
            this.PresetName = PresetName;
            Presets.ChangePreset(PresetName);
            this.CharacterName = CharacterName;
            this.ChatStarterUserID = ChatStarterUserID;

            if (!TextUI_Base.GetInstance().Cards.TryGetValue(this.CharacterName, out ProfileData value))
            {
                throw new Exception("Failed to load character profile data. This is likely a bug.");
            }
            this.ScenarioInfo = ScenarioInfo;
            CharacterProfile = value;
            CharacterProfile.Scenario = this.ScenarioInfo;
            Username = Client.GetInstance().FindUsername(ChatStarterUserID);
        }

        /// <summary>
        /// Default constructor for creating a new chat with an AI.
        /// The AI's greeting is automatically added here if the scenario is not a chat scenario.
        /// </summary>
        /// <param name="channelID">The Discord static channel ID #</param>
        /// <param name="charProfile">The character profile</param>
        /// <param name="presets">Default preset for the chat</param>
        /// <param name="username">Username who started the chat (so we don't have to call the client constantly. Username is not serialized.</param>
        public Chats(ulong channelID, ProfileData charProfile, TextUI_Presets.PresetEnum PresetName, string username, Scenario.ScenarioPresets scenario, ulong userID, string customScenario = "")
        {
            ChannelID = channelID;
            CharacterProfile = charProfile;
            Presets = new TextUI_Presets();
            this.PresetName = PresetName;
            Presets.ChangePreset(PresetName);
            CharacterName = CharacterProfile.CharacterName;
            Username = username;
            if (!string.IsNullOrEmpty(customScenario))
            {
                scenario = Scenario.ScenarioPresets.Default;
                CharacterProfile.Scenario = Scenario.GetScenario(scenario, charProfile.NickOrName(), customScenario);
            }
            else
                CharacterProfile.Scenario = Scenario.GetScenario(scenario, charProfile.NickOrName());
            ScenarioInfo = CharacterProfile.Scenario;
            if (scenario != Scenario.ScenarioPresets.Chatbot)
                AddMessage(CharacterProfile.NickOrName(), CharacterProfile.AllGreetings[0], CHARACTER_ID, CHARACTER_ID);
            ChatStarterUserID = userID;
        }


        /// <summary>
        /// This prints out the entire chat history. Necessary to submit to the neuro net.
        /// </summary>
        /// <param name="includeProfile">Whether to include the character profile or not</param>
        /// <returns></returns>
        public string PrintHistory(bool includeProfile = false)
        {
            StringBuilder sb = new();
            if (includeProfile)
                sb.AppendLine(CharacterProfile.ProfileInfo(Username));
            foreach (var data in ChatHistory)
            {
                sb.Append(data.Name).
                    Append(": ").
                    AppendLine(data.Message);
            }
            return sb.ToString().Replace("\r", string.Empty);
        }

        /// <summary>
        /// If the chat history is requested this will send it as an array, each index represents the character limit of a single message sent to Discord.
        /// So it will let you submit multiple messages back to back based on each index.
        /// </summary>
        /// <param name="includeProfile">If you want to include the character profile in the request</param>
        /// <returns>Chat history list sorted by character limit</returns>
        public override List<string> HistoryByCharacterLimit(bool includeProfile = false)
        {
            List<string> strings = [];
            int total = (includeProfile) ? CharacterProfile.ProfileInfo(Username).Length : 0;
            StringBuilder sb = new();
            foreach (var data in ChatHistory)
            {
                string format = data.Format();
                if (format.Length + sb.Length >= Discord_Character_Limit)
                {
                    total = (includeProfile) ? CharacterProfile.ProfileInfo(Username).Length : 0;
                    strings.Add(sb.ToString());
                    sb.Clear();
                }
                sb.AppendLine(format);
                total += format.Length;
            }
            return strings;
        }

    }
}
