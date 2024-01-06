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

        [Test]
        public async Task TestQueues()
        {
            var instance = TextUI_Base.GetInstance();

            for (int i = 0; i < 10; i++)
            {
                await instance.NeuroNetCalls.Enqueue(QTest);
            }
            Assert.That(instance.NeuroNetCalls.TotalInQueue, Is.EqualTo(0));
            Assert.That(tester, Is.EqualTo(10));
            await instance.NeuroNetCalls.Enqueue(QTest);
            await instance.NeuroNetCalls.Enqueue(QTest);
            await instance.NeuroNetCalls.Enqueue(QTest);
            Assert.That(tester, Is.EqualTo(13));
        }
        int tester = 0;
        private async Task QTest()
        {
            await Task.Delay(100);
            tester++;
        }
    }
}
