using Discord_AI_Presence.Stable_Diffusion;
using Discord_AI_Presence.Text_WebUI.Presets;
using Discord_AI_Presence.Text_WebUI.ProfileScripts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_AI_Presence.Text_WebUI
{
    public class TextUI_Base
    {
        public TextUI_Presets Presets { get; init; }
        public Dictionary<string, ProfileReader> Cards { get; } = new(StringComparer.OrdinalIgnoreCase);
        private static TextUI_Base _instance = null!;
        private static readonly object _lock = new();


        public TextUI_Base()
        {
            Presets = new TextUI_Presets();
            PopulateProfileDictionary();
        }   
        
        /// <summary>
        /// Singleton instance to use whenever data needs to be read for the Text AI
        /// </summary>
        /// <returns>The class instance</returns>
        public static TextUI_Base GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new TextUI_Base();
                }
            }
            return _instance;
        }

        /// <summary>
        /// Searches the dictionary of character profiles and returns the result.
        /// Dictionary is case insensitive by default.
        /// </summary>
        /// <param name="charFirstName">Search only by the character's first name</param>
        /// <returns>Returns null if nothing found.</returns>
        public static ProfileReader? MatchName(string charFirstName)
        {
            if (GetInstance().Cards.TryGetValue(charFirstName, out var charData))
            {
                return charData;
            }

            return null;
        }

        // Location of character profile cards using the datapath
        protected virtual string ProfileLocation(string filename = "")
        {
            if (!filename.IsSafePath())
                filename = string.Empty;
            else
                filename = string.Concat(filename, ".json");
            return $@"{Environment.CurrentDirectory}\TextUI_Files\AiProfileCards\{filename}";
        }

        // Character files stored in a folder using json format are grabbed and deserialized into the ProfileReader data class.
        private void PopulateProfileDictionary()
        {
            var di = new DirectoryInfo(ProfileLocation());
            var fi = di.GetFiles();
            foreach (var files in fi)
            {
                var fileData = File.ReadAllText(files.FullName);
                var jsonData = JsonConvert.DeserializeObject<ProfileReader>(fileData);
                if (jsonData != null)
                    Cards.Add(jsonData.CharacterName, jsonData);
            }
        }
    }
}
