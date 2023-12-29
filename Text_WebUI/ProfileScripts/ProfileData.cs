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

        [JsonProperty("world_scenario")]
        public string WorldScenario { get; init; }

        [JsonProperty("scenario")]
        public string Scenario { get; init; }

        [JsonProperty("char_name")]
        public string CharacterName { get; init; }

        [JsonProperty("char_nickname")]
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
        public string ProfileInfo()
        {
            var properties = GetType().GetProperties();
            var sb = new StringBuilder();
            foreach (var prop in properties)
            {
                if (prop.GetValue(this) == null || prop.GetValue(this).GetType() != typeof(string)
                    || prop.Name.Equals("CharacterIntroduction") || string.IsNullOrEmpty((string)prop.GetValue(this)))
                    continue;
                //prop.GetValue(this).GetType().Dump();
                sb.AppendLine($"{prop.Name}: ").
                    AppendLine((string)prop.GetValue(this)).AppendLine();
            }
            //sb.AppendLine($"{prop.Name}: ").Append(prop.GetValue(this));
            return sb.ToString().Replace("<START>", $"This is how {CharacterName} should speak");
        }

        public string ScenarioInfo
        {
            get
            {
                if (string.IsNullOrEmpty(WorldScenario))
                    return Scenario;
                return WorldScenario!;
            }
        }

        /// <summary>
        /// If the character has a nickname it will use that instead of their character name.
        /// </summary>
        /// <returns></returns>
        public string NickOrName()
        {
            return CharacterNickName ?? CharacterName;
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

        /// <summary>
        /// Character cards use placeholders of the character's name and user name to keep things dynamic. They are replaced with the correct information here.
        /// </summary>
        /// <param name="username">User who started the chat</param>
        public void ReplaceNames(string username)
        {
            var properties = GetType().GetProperties();
            foreach (var prop in properties)
            {
                if (prop.GetValue(this) != null && prop.GetValue(this).GetType() == typeof(string) && !string.IsNullOrEmpty(prop.GetValue(this).ToString()))
                    prop.SetValue(this, prop.GetValue(this).ToString().
                        Replace("{{char}}", NickOrName()).
                        Replace("{{user}}", username));
            }
            for(int i = 0; i < AllGreetings.Count; i++)
            {
                    AllGreetings[i] = AllGreetings[i].
                    Replace("{{char}}", NickOrName()).
                    Replace("{{user}}", username);
            }
        }
    }
}
