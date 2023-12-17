using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Discord_AI_Presence.Text_WebUI.Presets
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class TextUI_Presets
    {
        [JsonProperty]
        public PresetData? CurPreset { get; set; }

        /* Global presets are presets that don't impact the weights of the neuronet.
         * They impact the output after the weights have been set.*/
        private const string GlobalPreset = "GlobalPreset.json";

        /* Enum represents the exact filenames of each weight preset for easy searching.
         * But also necessary for the Discord slash command later to let the user choose presets.*/
        public enum PresetEnum
        {
            Asterism,
            BeamSearch,
            BigO,
            ContrastiveSearch,
            Default,
            Deterministic,
            DivineIntellect,
            KoboldGodlike,
            KoboldLiminalDrift,
            LLaMaPrecise,
            MidnightEnigma,
            Mirostat,
            Naive,
            NovelAIBestGuess,
            NovelAIDecadence,
            NovelAIGenesis,
            NovelAILycaenidae,
            NovelAIOuroboros,
            NovelAIPleasingResults,
            NovelAISphinxMoth,
            NovelAIStorywriter,
            Pygmalion,
            Shortwave,
            Simple1,
            SpaceAlien,
            StarChat,
            TFSwithTopA,
            Titanic,
            Yara
        }

        public TextUI_Presets()
        {
            ChangePreset(PresetEnum.Default);
        }

        /// <summary>
        /// Merges the global presets with the neuronet presets, then assigns it as the current preset.
        /// </summary>
        /// <param name="presetType"></param>
        public void ChangePreset(PresetEnum presetType)
        {
            var preset = JObject.Parse(PresetFiles(presetType.ToString()));
            var globalPreset = JObject.Parse(PresetFiles(GlobalPreset));
            preset.Merge(globalPreset, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            });
            CurPreset = JsonConvert.DeserializeObject<PresetData>(preset.ToString());
        }

        /// <summary>
        /// Fetches a json file and returns it as a string
        /// </summary>
        /// <param name="filename">Include the json extension</param>
        /// <returns>The contents of the file</returns>
        private string PresetFiles(string filename)
        {
            return File.ReadAllText(PresetsLocation(filename));
        }

        /// <summary>
        /// Fetch the location of the preset files.
        /// </summary>
        /// <param name="filename">Search for a file. Do not include the file extension or special characters.</param>
        /// <returns>The file directory or the file itself if a filename is specified</returns>
        protected virtual string PresetsLocation(string filename = "")
        {
            if (!filename.IsSafePath())
                filename = string.Empty;
            else
                filename = string.Concat(filename, ".json");
            return $@"{Environment.CurrentDirectory}\TextUI_Files\Preset_Files\{filename}";
        }
    }
}