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
        public string? Personality { get; init; }
        [NotNull]
        [JsonProperty("description")]
        public string CharacterDescription { get; init; }

        [JsonProperty("example_dialogue")]
        public string?ExampleDialogue { get; init; }

        [JsonProperty("world_scenario")]
        public string? WorldScenario { get; init; }

        [NotNull]
        [JsonProperty("char_name")]
        public string CharacterName { get; init; }

        [JsonProperty("char_nickname")]
        public string? CharacterNickName { get; init; }

        [JsonProperty("avatar")]
        public string? AvatarUrl { get; init; }

        [NotNull]
        [JsonProperty("first_mes")]
        public string CharacterIntroduction { get; init; }
        /// <summary>
        /// Some character profiles include alternative greetings for different roleplay starts.
        /// This List will also add the original greeting and be used to choose which greeting one wants.
        /// </summary>
        [JsonProperty("alternate_greetings")]
        public List<string> AllGreetings { get; init; }
        #endregion

        [JsonConstructor]
        public ProfileData(string CharacterIntroduction, List<string> AllGreetings, string CharacterName, string CharacterDescription)
        {
            this.CharacterName = CharacterName;
            this.CharacterDescription = CharacterDescription;
            this.CharacterIntroduction = CharacterIntroduction;
            AllGreetings ??= [];
            this.AllGreetings = AllGreetings;
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
        public string ChooseGreeting(int val = 0)
        {
            if (val < AllGreetings.Count)
                return AllGreetings[val];
            val = AllGreetings.Count - 1;
            return AllGreetings[val];
        }

        /// <summary>
        /// Prints out the value of all non-null or empty properties from the character profile.
        /// </summary>
        /// <returns></returns>
        public string ProfileInfo()
        {
            var properties = GetType().GetProperties();
            var sb = new StringBuilder();
            foreach (var prop in properties)
            {
                sb.AppendLine($"{prop.Name}: ").Append(prop.GetValue(this));
            }
            return sb.ToString();
        }

        /// <summary>
        /// If the character has a nickname it will use that instead of their character name.
        /// </summary>
        /// <returns></returns>
        public string NickOrName()
        {
            return CharacterNickName ?? CharacterName;
        }
    }
}
