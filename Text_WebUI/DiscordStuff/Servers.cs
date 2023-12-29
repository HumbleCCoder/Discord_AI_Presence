using Discord_AI_Presence.Text_WebUI.MemoryManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_AI_Presence.Text_WebUI.DiscordStuff
{
    public class TextUI_Servers
    {
        public ulong ServerID { get; init; }
        /// <summary>
        /// AI Chat settings for this server.
        /// </summary>
        public Settings ServerSettings { get; set; }
        /// <summary>
        /// Contains the AI chats taking place in this server.
        /// </summary>
        public Dictionary<ulong, Chats> AIChats;

        public TextUI_Servers(ulong ServerID, Settings ServerSettings, Dictionary<ulong, Chats> AIChats) =>
            (this.ServerID, this.ServerSettings, this.AIChats) = (ServerID, ServerSettings, AIChats);

    }
}