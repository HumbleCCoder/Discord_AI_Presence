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
        public TextUI_Presets Presets { get; init; }
        public ProfileData CharacterProfile { get; init; }
        /// <summary>
        /// Will not be serialized. This reduces client calls to get usernames.
        /// </summary>
        public string Username { get; init; }
        /// <summary>
        /// Instead of serializing entire character profiles, this will be used to call the character upon reload.
        /// </summary>
        [JsonProperty]
        public ulong ChannelID { get; init; }
        [JsonProperty]
        public string CharacterName { get; init; }
        /// <summary>
        /// Instead of serializing entire preset file, this will be used to simply call the preset.
        /// </summary>
        [JsonProperty]
        public TextUI_Presets.PresetEnum PresetName { get; init; }

        [JsonConstructor]
        public Chats(ulong channelID, TextUI_Presets.PresetEnum presets, string characterName)
        {
            ChannelID = channelID;
            Presets = new TextUI_Presets();
            Presets.ChangePreset(presets);
            CharacterName = characterName;
            if(!TextUI_Base.GetInstance().Cards.TryGetValue(characterName, out ProfileData value))
            {
                throw new Exception("Failed to load character profile data. This is likely a bug.");
            }
            CharacterProfile = value;
        }

        /// <summary>
        /// Default construtor for creating a new chat with an AI.
        /// </summary>
        /// <param name="channelID">The Discord static channel ID #</param>
        /// <param name="charProfile">The character profile</param>
        /// <param name="presets">Default preset for the chat</param>
        /// <param name="username">Username who started the chat (so we don't have to call the client constantly. Username is not serialized.</param>
        public Chats(ulong channelID, ProfileData charProfile, TextUI_Presets presets, string username)
        {
            ChannelID = channelID;
            CharacterProfile = charProfile;
            Presets = presets;
            CharacterName = CharacterProfile.CharacterName;
            CharacterProfile.ReplaceNames(username);
            Username = username;
        }
        
        /// <summary>
        /// This prints out the entire chat history. Necessary to submit to the neuro net.
        /// Username of the person who started the chat is used
        /// </summary>
        /// <param name="includeProfile">Whether to include the character profile or not</param>
        /// <returns></returns>
        public string PrintHistory(bool includeProfile = false)
        {
            StringBuilder sb = new();
            if(includeProfile)
                sb.AppendLine(CharacterProfile.ProfileInfo());
            foreach (var data in ChatHistory)
            {
                sb.Append(data.Name).
                    Append(": ").
                    AppendLine(data.Message);
            }
            return sb.ToString();
        }
    }
}
