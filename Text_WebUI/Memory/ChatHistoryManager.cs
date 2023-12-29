using Discord_AI_Presence.Text_WebUI.ProfileScripts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Discord_AI_Presence.Text_WebUI.MemoryManagement
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class ChatHistoryManager
    {
        public List<Memory> ChatHistory { get; protected set; } = [];
        /// <summary>
        /// Shaved 200 off the actual limit of 2000 to prevent issues.
        /// </summary>
        private const int Discord_Character_Limit = 1800;
        private const int MAX_TOKEN_SIZE = 2018;
        private const double TOKEN_MULTIPLIER = 3.701d;

        public Memory FirstMsg => ChatHistory[0];
        public int HistoryCount => ChatHistory.Count;

        /// <summary>
        /// Finds the current greetings being used.
        /// </summary>
        /// <param name="charProfile">The character profile which contains all greetings.</param>
        /// <returns>Returns the index value, otherwise -1 if not found.</returns>
        public int FindGreeting(ProfileData charProfile)
        {
            var index = charProfile.AllGreetings.FindIndex(x => x.Equals(ChatHistory[0].Message));
            return index;
        }

        /// <summary>
        /// Gets the entire word count of the entire message history including the username.
        /// </summary>
        public int Get_TotalWordCount
        {
            get
            {
                int counter = 0;
                foreach (var data in ChatHistory)
                {
                    counter += data.Message.Split(' ').Length + data.Name.Split(' ').Length;
                }
                return counter;
            }
        }

        /// <summary>
        /// This gets the character count of the entire message history including the username but it does not include the character profile.
        /// </summary>
        public int Get_TotalCharacterCount
        {
            get
            {
                var counter = 0;
                foreach (var data in ChatHistory)
                {
                    counter += data.Message.Length + data.Name.Length;
                }
                return counter;
            }
        }

        public ChatHistoryManager()
        {

        }

        /// <summary>
        /// In case a chat needs to be taken to a new channel you can simply swap the chat history.
        /// </summary>
        public void SwapChatHistory(List<Memory> chatHistory)
        {
            ChatHistory = chatHistory;
        }

        /// <summary>
        /// Simply adds message to the chat history
        /// </summary>
        /// <param name="username">Name of the Discord user</param>
        /// <param name="message">Message sent to Discord</param>
        /// <param name="userID">Discord static User ID</param>
        public void AddMessage(string username, string message, ulong userID, ulong msgID)
        {
            ChatHistory.Add(new Memory(message, username, userID, msgID));
        }

        /// <summary>
        /// Trims the chat history.
        /// </summary>
        /// <param name="trimAmt">The amount of entries to trim.</param>
        /// <returns>The count of the chat history array after the trim.</returns>
        public int TrimChatHistory(int trimAmt)
        {
            ChatHistory.RemoveRange(0, trimAmt);
            return ChatHistory.Count;
        }

        /// <summary>
        /// Returns the token count. AI neuro net removes from the leftmost position with their automatic trimming process. Unfortunately this can often trim character profiles.
        /// It is possible to submit the character profile after the chat history, to let the auto trimming work on the history only, but the AI may say a lot of nonsense
        /// reducing the quality of chat if the history isn't last.
        /// </summary>
        /// <param name="charProfile">Character profile data should always be in the chat history and never trimmed..</param>
        /// <returns>Token count of message history and character profile.</returns>
        public int Chat_TotalTokens(string charProfile)
        {
            return (int)((Get_TotalCharacterCount + charProfile.Length) / TOKEN_MULTIPLIER);
        }
    }
}
