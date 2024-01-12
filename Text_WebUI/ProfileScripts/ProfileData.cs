using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_AI_Presence.Text_WebUI.ProfileScripts
{
    public class ProfileData
    {
        #region properties
        [JsonProperty("personality")]
        public string Personality { get; init; }
        [JsonProperty("description")]
        public string CharacterDescription { get; init; }

        [JsonProperty("example_dialogue")]
        public string ExampleDialogue { get; init; }

        [JsonProperty("scenario")]
        public string Scenario { get; set; }

        [JsonProperty("name")]
        public string Name { get; init; }

        [JsonProperty("nickname")]
        public string CharacterNickName { get; init; }

        [JsonProperty("avatar")]
        public string AvatarUrl { get; init; }
        /// <summary>
        /// Some character profiles include alternative greetings for different roleplay starts.
        /// This List will also add the original greeting and be used to choose which greeting one wants.
        /// </summary>
        [JsonProperty("alternate_greetings")]
        public List<string> AllGreetings { get; init; }

        [JsonProperty("first_mes")]
        public string CharacterIntroduction { get; init; }
        #endregion

        [JsonConstructor]
        private ProfileData(string Scenario)
        {
            this.Scenario = Scenario;
            AllGreetings ??= [];
            if (!string.IsNullOrEmpty(this.CharacterIntroduction))
            {
                AllGreetings.Insert(0, CharacterIntroduction);
            }
        }

        /// <summary>
        /// Defaults to the first greeting if no value is chosen
        /// </summary>
        /// <param name="val">Leave at default value if using the initial greeting</param>
        /// <returns>The chosen greeting.</returns>
        public string ChooseGreeting(string username, int val = 0)
        {
            if (val < AllGreetings.Count)
                return AllGreetings[val];
            val = AllGreetings.Count - 1;
            return AllGreetings[val].
                Replace("{{char}}", NickOrName()).
                Replace("{{user}}", username);
        }

        /// <summary>
        /// Prints out the value of all non-null or empty properties from the character profile.
        /// </summary>
        /// <returns></returns>
        public string ProfileInfo(string username)
        {
            var properties = GetType().GetProperties();
            var sb = new StringBuilder();
            foreach (var prop in properties)
            {
                if (prop.GetValue(this) == null || prop.GetValue(this).GetType() != typeof(string)
                    || string.IsNullOrEmpty((string)prop.GetValue(this)))
                    continue;
                sb.AppendLine($"{prop.Name}: ").
                    AppendLine((string)prop.GetValue(this)).AppendLine();
            }
            return sb.ToString()
                .Replace("<START>", $"This is how {Name} should speak")
                .Replace("{{char}}", NickOrName())
                .Replace("{{user}}", username);
        }

        /// <summary>
        /// If the character has a nickname it will use that instead of their character name.
        /// This includes the key file for the profile dictionary.
        /// </summary>
        /// <returns></returns>
        public string NickOrName()
        {
            return CharacterNickName ?? Name;
        }

        public int GreetingsIterator(int curIndex)
        {
            if (curIndex == -1)
                return curIndex;
            curIndex++;
            if (curIndex == AllGreetings.Count)
                return 0;
            return curIndex;
        }

        public void ChangeScenario(string scenario)
        {
            this.Scenario = scenario;
        }
    }
}
