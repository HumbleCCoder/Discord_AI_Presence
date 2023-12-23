using Discord_AI_Presence.Text_WebUI;
using Discord_AI_Presence.Text_WebUI.Presets;

namespace UnitTesting
{    
    internal class TEST_TextUI_Presets
    {
        [Test]
        public void TestPresetLocation()
        {
            Assert.That(TextUI_Base.GetInstance().Presets.CurPreset!.do_sample, Is.True);
        }
    }
}
