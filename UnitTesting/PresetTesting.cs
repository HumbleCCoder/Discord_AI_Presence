using Discord_AI_Presence.Text_WebUI.Presets;

namespace UnitTesting
{
    internal class PresetTestingHelper : TextUI_Presets
    {
        protected override string PresetsLocation(string filename = "")
        {
            string solutionDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..");
            return Path.Combine(solutionDirectory, @"bin\Debug\net7.0\TextUI_Files\Preset_Files").Replace("UnitTesting", string.Empty);
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
