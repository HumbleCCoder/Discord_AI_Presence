using Discord_AI_Presence.Text_WebUI.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_AI_Presence.Text_WebUI.DiscordStuff
{
    /*
     * No AI Channels means that AI cannot be called out by anyone. AI can come out in all channels not set at NoAiChannels.
     * Dedicated AI Channels are roleplay channels with default introductions.
     */
    public class Settings
    {
        /// <summary>
        /// A dedicated AI channel is a channel that can reload chat history from if the bot is closed.
        /// These channels will be roleplay channels and, as such, use character's default intro messages.
        /// </summary>
        public List<ulong> DedicatedAIChannels { get; set; } = [];
        /// <summary>
        /// A channel where AI will never come out if called.
        /// </summary>
        public List<ulong> NoAIChannels { get; set; } = [];
        /// <summary>
        /// What are you using to tell Discord you are submitting a bot command? You don't want your bot commands being submitted to the AI memory or the AI starts to act weird.
        /// </summary>
        public string BotCommandTrigger { get; set; }
        /// <summary>
        /// If you want to use your own custom webhook specifically for the AI add the Webhook URL here. Otherwise it will default to the first Webhook on your list.
        /// </summary>
        public string Webhooks { get; set; }
        /// <summary>
        /// Allow AI to come out randomly in non dedicated channels where they are permitted to.
        /// </summary>
        public bool AllowRandomAIOccurance { get; set; } = false;        
        /// <summary>
        /// If the AI is allowed to come out randomly, this is the chance of that happening.
        /// </summary>
        public double randomAppearanceThreshold = 0.001;
        /// <summary>
        /// Presets are what you change to influence what comes out of the neuro net. Mirostat and Universal Creative, for example, will be more creative, but do a lot of narrating too.
        /// </summary>
        public TextUI_Presets.PresetEnum DefaultPreset { get; set; } = TextUI_Presets.PresetEnum.UniversalLight;
        public Settings() { }
    }
}
