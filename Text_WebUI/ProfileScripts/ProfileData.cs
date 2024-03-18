using Discord_AI_Presence.Text_WebUI.Instructions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        #region Json properties
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
        /* Although nicknames are not something found in character profile cards
        /* a "nickname": property can be added to the json file anywhere if you want the character
        /* to have a nickname.
         */
        [JsonProperty("nickname")]
        public string CharacterNickName { get; init; }
        /// <summary> This is a required property or Discord will throw an error when loading the webhook. </summary>
        [JsonProperty("avatar")]
        public string AvatarUrl { get; init; }
        [JsonProperty("first_mes")]
        public string CharacterIntroduction { get; set; }
        #endregion        

        /// <summary>
        /// Copies the class as a new object so the global data is unchanged.
        /// </summary>
        /// <param name="originalMessage">The message sent through Discord that summoned the AI</param>
        /// <param name="preset">The preset data as a reference to be set here.</param>
        /// <returns>A copy of the requested Character Profile data</returns>
        public ProfileData GetNewInstance(string originalMessage, ref string preset)
        {
            // scenario, preset, firstmes
            var parameters = new ChatParameters(NickOrName(), originalMessage);
            var jObj = JsonConvert.SerializeObject(this);
            var newObj = JsonConvert.DeserializeObject<ProfileData>(jObj);
            newObj.Scenario = parameters.ApplyParamData(Scenario);
            preset = parameters.ApplyParamData(preset);
            newObj.CharacterIntroduction = parameters.ApplyParamData(CharacterIntroduction);
            return newObj;
        }

        /// <summary>
        /// Prints out the value of all non-null or empty properties from the character profile.
        /// </summary>
        /// <returns>A concacted string containing all of the information from this class.</returns>
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
        /// <returns>Returns the nickname if available, otherwise the name of the character</returns>
        public string NickOrName()
        {
            return CharacterNickName ?? Name;
        }

        /// <summary>
        /// Simply changes the scenario.
        /// </summary>
        /// <param name="scenario">The scenario</param>
        public void ChangeScenario(string scenario)
        {
            Scenario = scenario;
        }

        // Any property not set as init needs a constructor for the Json to load the data.
        [JsonConstructor]
        private ProfileData(string Scenario, string CharacterIntroduction)
        {
            this.Scenario = Scenario;
        }
    }
}
