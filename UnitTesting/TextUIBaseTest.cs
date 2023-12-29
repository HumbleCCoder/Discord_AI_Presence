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

    internal class ProfileTests
    {
        [Test]
        public void CharacterJsonTest()
        {
            Assert.That(TextUI_Base.GetInstance().Cards, Is.Not.Empty);
            Assert.That(TextUI_Base.GetInstance().Cards.ElementAt(0).Value.CharacterName, Is.Not.Empty);
        }
    }
}
