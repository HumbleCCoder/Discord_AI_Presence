using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Discord_AI_Presence.Text_WebUI.Presets
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public class TextUI_Presets
    {
        [JsonProperty]
        public PresetData? CurPreset { get; private set; }
        private const string DefaultPreset = "Default.json";
        private const string GlobalPreset = "GlobalPreset.json";
        private readonly string DefaultDirectory;
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

        public TextUI_Presets(string defaultDirectory = "")
        {
            string solutionDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, defaultDirectory);
            DefaultDirectory = Path.Combine(solutionDirectory, "Text_WebUI\\Presets\\Preset_Files");
            ChangePreset(PresetEnum.Default);
        }

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
        /// <param name="filename"></param>
        /// <returns></returns>
        private string PresetFiles(string filename)
        {
            var di = new DirectoryInfo(PresetsLocation());
            var files = di.GetFiles();
            var fileContent = files.First(file => file.Name.Equals(filename, StringComparison.OrdinalIgnoreCase) ||
            file.Name.Equals(DefaultPreset, StringComparison.OrdinalIgnoreCase));
            return File.ReadAllText(fileContent.FullName);
        }

        /// <summary>
        /// Fetch the location of the preset files. Specify filename to get a specific file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public virtual string PresetsLocation(string filename = "")
        {
            return $@"{Environment.CurrentDirectory}\Text_WebUI\Presets\Preset_Files\{filename}";
        }
    }
}