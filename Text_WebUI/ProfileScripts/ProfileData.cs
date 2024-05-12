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
    /// <summary>
    /// Gotta convert this to a value type and make a different class altogether to handle the data of non-profile stuff
    /// </summary>
    public struct ProfileData
    {
        #region Json properties
        [JsonProperty("personality")]
        public string Personality { get; init; }
        [JsonProperty("description")]
        public string CharacterDescription { get; init; }

        [JsonProperty("example_dialogue")]
        public string ExampleDialogue { get; init; }

        [JsonProperty("scenario")]
        public string Scenario { get; init; }

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
        public string CharacterIntroduction { get; init; }
        #endregion        

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

        // Any property not set as init needs a constructor for the Json to load the data.
        public ProfileData()
        {
        }
    }
}
