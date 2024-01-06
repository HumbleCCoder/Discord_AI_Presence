using Discord_AI_Presence.Text_WebUI.MemoryManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_AI_Presence.Text_WebUI.DiscordStuff
{
    public class TextUI_Servers(ulong ServerID)
    {
        public ulong ServerID { get; init; } = ServerID;
        /// <summary>
        /// AI Chat settings for this server.
        /// </summary>
        public Settings ServerSettings { get; set; } = new();
        /// <summary>
        /// Contains the AI chats taking place in this server.
        /// </summary>
        public List<Chats> AIChats { get; init; } = [];

        public Chats FindChat(ulong channeID) => AIChats.FirstOrDefault(x => x.ChannelID == channeID);

        public void ChangeSettings(Settings newSettings) => ServerSettings = newSettings;
        /// <summary>
        /// Gets the message by the ID number. Checks if the message ID is > 0 meaning it returned a default struct if it's 0;
        /// </summary>
        /// <param name="MsgID">Discord message ID number</param>
        /// <returns>Returns null if not found.</returns>
        public Chats FindChatByMsgID(ulong MsgID) => AIChats.FirstOrDefault(x => x.FindMessageByID(MsgID).MsgID > 0);


        public void StartChat(Chats chat)
        {
            AIChats.Add(chat);
        }

        public void EndChat(string charName)
        {
            var index = AIChats.FindIndex(x => x.CharacterName.Equals(charName, StringComparison.OrdinalIgnoreCase));
            AIChats.RemoveAt(index);
        }

        /// <summary>
        /// There is no support for multiple AI chatting in the same channel right now because of 
        /// Discord webhook limitations would cause rate limiting to happen. There is a way to handle it but I don't think this feature is necessary for now.
        /// This makes sure it won't start a new chat if a current chat is happening.
        /// </summary>
        /// <param name="channelId">The unique Discord channel ID number</param>
        /// <returns>True if a current AI chat is happening in the channel</returns>
        public bool AiChatExists(ulong channelId)
        {
            return AIChats.FirstOrDefault(x => x.ChannelID == channelId) != null;
        }

        /// <summary>
        /// This method is to prevent the same characters from existing in a channel at the same time.
        /// </summary>
        /// <param name="charName">Character name</param>
        /// <returns>Returns not null if the character exists in the channel already. Otherwise not null if they do not exist.</returns>
        public bool ExistsInChannel(string charName)
        {
            if (AIChats == null) return false;
            return AIChats.FirstOrDefault(x => x.CharacterName.Equals(charName, StringComparison.OrdinalIgnoreCase)) != null;
        }
    }
}