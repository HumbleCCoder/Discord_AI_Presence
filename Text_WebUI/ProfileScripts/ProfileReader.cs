using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_AI_Presence.Text_WebUI.ProfileScripts
{
    public class ProfileReader
    {
        #region properties
        [JsonProperty("personality")]
        public string Personality { get; set; }

        [JsonProperty("description")]
        public string CharacterDescription { get; set; }

        [JsonProperty("mes_dialogue")]
        public string ExampleDialogue { get; set; }

        [JsonProperty("world_scenario")]
        public string WorldScenario { get; set; }

        [JsonProperty("char_name")]
        public string CharacterName { get; set; }
        [JsonProperty("char_nickname")]
        public string CharacterNickName { get; set; }

        [JsonProperty("avatar")]
        public string AvatarUrl { get; set; }

        [JsonProperty("first_mes")]
        public string CharacterIntroduction { get; set; }
        #endregion
        public string NickOrName()
        {
            return string.IsNullOrEmpty(CharacterNickName) ? CharacterNickName : CharacterName;
        }
    }
}
