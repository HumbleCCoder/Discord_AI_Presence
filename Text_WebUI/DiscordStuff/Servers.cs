using Discord.WebSocket;
using Discord_AI_Presence.Text_WebUI.Button_Related;
using Discord_AI_Presence.Text_WebUI.MemoryManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
        /// The key is the channel ID.
        /// </summary>
        public Dictionary<ulong, Chats> AIChats { get; init; } = [];

        /// <summary>
        /// For picking a character if a duplicate is found.
        /// First button should be the next character button. Second should pick a character
        /// </summary>
        internal Dictionary<ulong,ButtonHandling> DuplicateHandling { get; init; } = new();

        /// <summary>
        /// Find chat by channel ID.
        /// </summary>
        /// <param name="channelId">The Discord channel ID</param>
        /// <param name="chatStarterId">The user ID of the person who started the chat</param>
        /// <returns>Null if not found, otherwise the chat</returns>
        public Chats FindChat(ulong channelId)
        {
            if (!AIChats.TryGetValue(channelId, out var chats))
                return null;

            return chats;
        }

        public void ChangeSettings(Settings newSettings) => ServerSettings = newSettings;

        /// <summary>
        /// Gets the message by the ID number.;
        /// </summary>
        /// <param name="MsgID">Discord message ID number</param>
        /// <param name="channelId">Discord channel ID number</param>
        /// <returns>Returns a default state struct if not found.</returns>
        public Memory FindMsgByMsgID(ulong MsgID, ulong channelId)
        {
            if (!AIChats.TryGetValue(channelId, out var chats))
                return default;

            if (!chats.ChatHistory.TryGetValue(MsgID, out var msg))
                return default;

            return msg;
        }


        public Chats StartChat(Chats chat)
        {
            if (AIChats.TryGetValue(chat.ChannelID, out var curChat))
                return curChat;
            AIChats.Add(chat.ChannelID, chat);
            return AIChats[chat.ChannelID];
        }

        public bool EndChat(ulong channelID, bool end)
        {
            if (!end)
                return false;
            AIChats.Remove(channelID);
            return true;
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
            return AIChats.ContainsKey(channelId);
        }
    }
}
