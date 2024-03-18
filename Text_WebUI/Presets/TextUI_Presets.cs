using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Discord_AI_Presence.Text_WebUI.Presets
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class TextUI_Presets
    {
        public JObject CurPreset { get; set; }
        private readonly string PresetsLocation = $@"{Environment.CurrentDirectory}\TextUI_Files\Preset_Files\";
        public const string DefaultPreset = "UniversalLight";
        /* Global presets are presets that don't impact the weights of the neuronet.
         * They impact the output after the weights have been set.*/

        /* Enum represents the exact filenames of each weight preset for easy searching.
         * But also necessary for the Discord slash command later to let the user choose presets.
         * Discord only allows 25 maximum presets in the choices for a slash command. Had to cut some presets.
         */
        public enum PresetEnum
        {
            Asterism,
            BeamSearch,
            BigO,
            ContrastiveSearch,
            Deterministic,
            DivineIntellect,
            Global,
            LLaMaPrecise,
            MidnightEnigma,
            Mirostat,
            MiroBronze,
            MiroGold,
            MiroSilver,
            Naive,
            Pygmalion,
            Shortwave,
            Simple1,
            SpaceAlien,
            StarChat,
            TFSwithTopA,
            Titanic,
            UniversalCreative,
            UniversalLight,
            UniversalSuperCreative,
            Yara
        }

        public TextUI_Presets()
        {
            ChangePreset(PresetEnum.UniversalLight);
        }

        /// <summary>
        /// Merges the global presets with the neuronet presets, then assigns it as the current preset.
        /// </summary>
        /// <param name="presetType"></param>
        public void ChangePreset(PresetEnum presetType)
        {
            var preset = JObject.Parse(PresetFiles(presetType));
            var globalPreset = JObject.Parse(PresetFiles(PresetEnum.Global));
            globalPreset.Merge(preset, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            });
            CurPreset = JsonConvert.DeserializeObject<JObject>(globalPreset.ToString());
        }

        /// <summary>
        /// Fetches a json file and returns it as a string
        /// </summary>
        /// <param name="file">The type of Preset File</param>
        /// <param name="returnOnlyLocation">If you only want the file path without the file itself</param>
        /// <returns>Either the path or the contents of the file</returns>
        private string PresetFiles(PresetEnum file, bool returnOnlyLocation = false)
        {
            if (returnOnlyLocation)
            {
                return PresetsLocation;
            }
            return File.ReadAllText($"{PresetsLocation}{file}.json");
        }        
    }
}
