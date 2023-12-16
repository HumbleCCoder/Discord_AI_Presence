using Discord_AI_Presence.Text_WebUI;
using Discord_AI_Presence.Text_WebUI.Presets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting
{
    internal class PresetTesting : TextUI_Presets
    {
        [Test]
        public void TestPresetLocation()
        {
            Assert.That(CurPreset.do_sample, Is.True);
        }

        public override string PresetsLocation(string filename = "")
        {
            string solutionDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..");
            return Path.Combine(solutionDirectory, "Text_WebUI\\Presets\\Preset_Files", filename).Replace("UnitTesting", string.Empty);
        }
    }
}
