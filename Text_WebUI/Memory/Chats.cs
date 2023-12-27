using Discord_AI_Presence.Text_WebUI.ProfileScripts;
using System.Text;

namespace Discord_AI_Presence.Text_WebUI.MemoryManagement
{
    /// <summary>
    /// Thread IDs are not the same as channel IDs. Check whether it's a thread or not and assign the right ID.
    /// </summary>        
    public class Chats : ChatHistoryManager
    {
        public ulong ChannelID { get; init; }
        public ProfileData CharacterProfile { get; init; }


        /// <summary>
        /// In case a chat needs to be taken a new channel you can simply swap the chat history.
        /// </summary>
        /// <param name="channelID">The Discord static channel ID #</param>
        /// <param name="charProfile">The character profile</param>
        /// <param name="memorySwap">Previous chat history</param>
        public Chats(ulong channelID, ProfileData charProfile, Dictionary<string, Memory> memorySwap)
        {
            this.ChannelID = channelID;
            this.CharacterProfile = charProfile;
        }

        /// <summary>
        /// Default construtor for creating a new chat with an AI.
        /// </summary>
        /// <param name="channelID">The Discord static channel ID #</param>
        /// <param name="charProfile">The character profile</param>
        public Chats(ulong channelID, ProfileData charProfile)
        {
            this.ChannelID = channelID;
            this.CharacterProfile = charProfile;
        }

        /// <summary>
        /// This prints out the entire chat history. Necessary to submit to the neuro net.
        /// </summary>
        /// <returns>Chat history in the structure of (Username): (Message)\n</returns>
        public string PrintHistory()
        {
            StringBuilder sb = new();

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
