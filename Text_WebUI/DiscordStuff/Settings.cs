using Discord_AI_Presence.Text_WebUI.Presets;
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
        public List<ulong> DedicatedAIChannels { get; set; } = [];
        /// <summary>
        /// A channel where AI will never come out if called.
        /// </summary>
        public List<ulong> NoAIChannels { get; set; } = [];
        /// <summary>
        /// Allow AI to come out randomly in non dedicated channels where they are permitted to.
        /// </summary>
        public bool AllowRandomAIOccurance { get; set; } = false;
        /// <summary>
        /// Defaults to Mirostat as I've found it to deliver more consistent quality but can cause problems with some characters.
        /// </summary>
        public TextUI_Presets.PresetEnum DefaultPreset { get; set; } = TextUI_Presets.PresetEnum.Mirostat;
        public Settings() { }
    }
}
