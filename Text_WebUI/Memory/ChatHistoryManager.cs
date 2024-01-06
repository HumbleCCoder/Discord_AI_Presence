using Discord_AI_Presence.Text_WebUI.DiscordStuff;
using Discord_AI_Presence.Text_WebUI.ProfileScripts;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Discord_AI_Presence.Text_WebUI.MemoryManagement
{
    /// <summary>
    /// 
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class ChatHistoryManager
    {
        public List<Memory> ChatHistory { get; protected set; } = [];
        /// <summary>
        /// Shaved 200 off the actual limit of 2000 to prevent issues.
        /// </summary>
        protected const int Discord_Character_Limit = 1800;
        protected const int MAX_TOKEN_SIZE = 2018;
        protected const double TOKEN_MULTIPLIER = 3.701d;

        public Memory FirstMsg => ChatHistory[0];
        public int HistoryCount => ChatHistory.Count;

        public abstract List<string> HistoryByCharacterLimit(bool includeProfile = false);

        /// <summary>
        /// Find the message by its unique Discord ID number.
        /// </summary>
        /// <param name="msgID">The Discord message ID number</param>
        /// <returns>Returns a default struct if message is not found.</returns>
        public Memory FindMessageByID(ulong msgID) => ChatHistory.FirstOrDefault(x => x.MsgID == msgID);
        /// <summary>
        /// Find the user by their unique Discord ID number.
        /// </summary>
        /// <param name="userID">The Discord message ID number</param>
        /// <returns>Returns a default struct if message is not found.</returns>
        public Memory FindMessageByUserID(ulong userID) => ChatHistory.FirstOrDefault(x => x.UserID == userID);

        /// <summary>
        /// Gets all chat participants.
        /// </summary>
        /// <returns>Returns chat participants without duplicates.</returns>
        public List<string> Participants() 
        {
            HashSet<string> result = [];
            foreach (var g in ChatHistory)
            {
                result.Add(g.Name);
            }
            return result.ToList();
        }

        /// <summary>
        /// Removes message from chat history
        /// </summary>
        /// <param name="byIndex">Removes by exact index</param>
        public void RemoveMessage(int byIndex)
        {
            ChatHistory.RemoveAt(byIndex);
        }

        /// <summary>
        /// Removes message from chat history
        /// </summary>
        /// <param name="byString">Search by matching string</param>
        public void RemoveMessage(string byString)
        {
            int index = ChatHistory.FindIndex(x => x.Message.Equals(byString, StringComparison.OrdinalIgnoreCase));
            ChatHistory.RemoveAt(index);
        }

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

        public ChatHistoryManager() { }

        /// <summary>
        /// Swap the chat history. Useful for switching to a new channel or reloading data on bot restart.
        /// </summary>
        /// <param name="chatHistory">The new chat history</param>
        public void SwapChatHistory(List<Memory> chatHistory)
        {
            ChatHistory = chatHistory;
        }

        /// <summary>
        /// Simply adds message to the chat history
        /// </summary>
        /// <param name="username">Name of the Discord user</param>
        /// <param name="message">Message sent to Discord</param>
        /// <param name="userID">Discord static User ID. AI ID is 000000</param>
        /// <param name="serverSettings">To check if a message should be sent to memory add the settings here, otherwise null.</param>
        public void AddMessage(string username, string message, ulong userID, ulong msgID, Settings serverSettings = null)
        {
            if(serverSettings != null)
            {
                if (!AllowMemorySubmission(message, serverSettings.BotCommandTrigger))
                    return;
            }
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
        /// Returns the token count. AI neuro net removes from the leftmost position with their automatic trimming process. 
        /// Unfortunately this can often trim character profiles.
        /// It is possible to submit the character profile after the chat history, to let the auto trimming work on the history only, 
        /// but the AI may say a lot of nonsense reducing the quality of chat if the history isn't last.
        /// </summary>
        /// <param name="charProfile">Character profile data should always be in the chat history and never trimmed.</param>
        /// <returns>Token count of message history and character profile.</returns>
        public int Chat_TotalTokens(string charProfile)
        {
            return (int)((Get_TotalCharacterCount + charProfile.Length) / TOKEN_MULTIPLIER);
        }

        /// <summary>
        /// Some checks to make sure things aren't being submitted to memory that might negatively affect AI quality.
        /// </summary>
        /// <param name="msg">the message to check</param>
        /// <param name="cmdTrigger">the discord bot's command trigger</param>
        /// <returns></returns>
        private static bool AllowMemorySubmission(string msg, string cmdTrigger)
        {
            if (msg.Length > 2 && msg[0] == '`' && msg[^1] == '`')
                return false; //Allows ` and ` opening/closing to not trigger AI responses or add to memory.
            if (msg.Length > cmdTrigger.Length && msg.Substring(0, cmdTrigger.Length).Equals(cmdTrigger))
                return false; //bot commands won't be sent to memory.
            if (msg.Length > 3 && msg[0] == '<' && msg[1] == ':' && msg[^1] == '>')
                return false; // Custom emojis won't be submitted to memory
            return true;
        }
    }
}
