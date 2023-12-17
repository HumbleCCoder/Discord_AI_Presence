using Discord_AI_Presence.Text_WebUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting
{
    internal class HelperClass : TextUI_Base
    {
        
    }
    internal class TextUIBaseTest
    {
        [Test]
        public void ConstructorTest()
        {
            Assert.That(TextUI_Base.GetInstance().Presets != null);
        }
    }
}
