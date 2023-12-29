using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_AI_Presence.Text_WebUI.DiscordStuff
{
    public class Settings
    {
        /// <summary>
        /// A dedicated AI channel is a channel that can reload chat history from if the bot is closed.
        /// </summary>
        public List<ulong> DedicatedAIChannels = [];
        /// <summary>
        /// A channel where AI will never come out if called.
        /// </summary>
        public List<ulong> NoAIChannels = [];
        /// <summary>
        /// Allow AI to come out randomly in non dedicated channels where they are permitted to.
        /// </summary>
        public bool AllowRandomAIOccurance = false;
        public Settings() { }
    }
}
