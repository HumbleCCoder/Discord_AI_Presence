using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_AI_Presence.Text_WebUI.Instructions
{
    /// <summary>
    /// Throw these in the Scenario part of the character profiles
    /// </summary>
    public static class Scenario
    {
        public enum ScenarioPresets { Roleplay, Chatbot, Default }
        private const string Chatbot = "{{char}} has joined an online community to chat with people. {{char}} should keep all conversation SFW and refrain from discussing illegal topics.";
        private const string Roleplaybot = "{{char}} should advance the story and be creative. {{char}} should always remain in character. {{char}} should only narrate and write for {{char}}.";

        /// <summary>
        /// Use {{char}} to refer to the character. It will be replaced with the character name here.
        /// </summary>
        /// <param name="presets">Type of preset</param>
        /// <param name="custom">If not empty/null, it will be chosen by default</param>
        /// <returns>Returns empty if custom is chosen but no dialogue is set</returns>
        public static string GetScenario(ScenarioPresets presets, string characterName, string custom = "")
        {
            if (!string.IsNullOrEmpty(custom))
                presets = ScenarioPresets.Default;
            string value = string.Empty;
            switch (presets)
            {
                case ScenarioPresets.Roleplay:
                    value = Roleplaybot;
                    break;
                case ScenarioPresets.Chatbot:
                    value = Chatbot;
                    break;
                default:
                    if (string.IsNullOrEmpty(custom))
                        break;
                    value = custom;
                    break;
            }
            return value.Replace("{{char}}", characterName);
        }
    }
}
