using Discord_AI_Presence.Text_WebUI.Presets;

namespace UnitTesting
{
    internal class PresetTestingHelper : TextUI_Presets
    {
        public override string PresetsLocation(string filename = "")
        {
            string solutionDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..");
            return Path.Combine(solutionDirectory, "Text_WebUI\\Presets\\Preset_Files", filename).Replace("UnitTesting", string.Empty);
        }
    }
    internal class TEST_TextUI_Presets
    { 
        [Test]
        public void TestPresetLocation()
        {
            PresetTestingHelper preset = new();
            Assert.That(preset.CurPreset.do_sample, Is.True);
        }        
    }
}
