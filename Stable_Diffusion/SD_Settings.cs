using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Discord_AI_Presence.Stable_Diffusion
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class SD_Settings
    {
        public List<string> Checkpoints { get; init; } = new List<string>();
        public SD_Settings()
        {

        }
    }
}
