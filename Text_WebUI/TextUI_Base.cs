using Discord_AI_Presence.Text_WebUI.DiscordStuff;
using Discord_AI_Presence.Text_WebUI.ProfileScripts;
using Newtonsoft.Json;

namespace Discord_AI_Presence.Text_WebUI
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class TextUI_Base
    {
        [JsonProperty]
        public List<TextUI_Servers> ServerData { get; init; } = [];
        public Dictionary<string, ProfileData> Cards { get; } = new(StringComparer.OrdinalIgnoreCase);
        private static TextUI_Base _instance = null!;
        private static readonly object _lock = new();

        public TextUI_Base()
        {            
            PopulateProfileDictionary();
        }

        [JsonConstructor]
        public TextUI_Base(List<TextUI_Servers> ServerData)
        {
            this.ServerData = ServerData;
            _instance = this;
        }

        public ProfileData GetAI(string name)
        {
            if (!Cards.TryGetValue(name, out ProfileData value))
                return null;
            return value;
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
        public static ProfileData MatchName(string charFirstName)
        {
            if (GetInstance().Cards.TryGetValue(charFirstName, out var charData))
            {
                return charData;
            }

            return null;
        }

        /// <summary>
        /// Location of character profile cards using the datapath
        /// </summary>
        /// <param name="filename">Do not add any special characters or the file extension</param>
        /// <returns></returns>
        protected virtual string ProfileLocation(string filename = "")
        {
            if (!filename.IsSafePath())
                filename = string.Empty;
            else
                filename = string.Concat("\\", filename, ".json");
            return $@"{Environment.CurrentDirectory}\TextUI_Files\AiProfileCards{filename}";
        }

        // Character files stored in a folder using json format are grabbed and deserialized into the ProfileData data class.
        private void PopulateProfileDictionary()
        {
            var di = new DirectoryInfo(ProfileLocation());
            var fi = di.GetFiles();
            foreach (var files in fi)
            {
                var fileData = File.ReadAllText(files.FullName);
                var jsonData = JsonConvert.DeserializeObject<ProfileData>(fileData);
                if (jsonData != null)
                    Cards.Add(jsonData.CharacterName, jsonData);
            }
        }
    }
}