using Discord_AI_Presence.Text_WebUI;
using Discord_AI_Presence.Text_WebUI.DiscordStuff;
using Discord_AI_Presence.Text_WebUI.MemoryManagement;
using Discord_AI_Presence.Text_WebUI.Presets;

namespace UnitTesting
{
    internal class ChatHistory_Testing
    {
        [Test]
        public void HistoryTest()
        {
            TextUI_Servers ser = new(0000000);
            ser.StartChat(new Chats(0543, TextUI_Base.GetInstance().Cards.First().Value.First(), ser.ServerSettings.DefaultPreset, "BobTheTester", Discord_AI_Presence.Text_WebUI.Instructions.Scenario.ScenarioPresets.Roleplay, 9023));
            var chats = ser.AIChats.Last().Value;
            chats.AddMessage("Test1", "A test is being conducted1", 777, 900);
            chats.AddMessage("Test2", "A test is being conducted2", 778, 901);
            chats.AddMessage("Test3", "A test is being conducted3", 779, 902);
            chats.AddMessage("Test4", "A test is being conducted4", 780, 903);
            for (int i = 1; i < chats.ChatHistory.Count; i++)
            {
                //Assert.Warn("This is I: " + i.ToString() + $" || Test{i}\n{chats.ChatHistory[i].Name}");
                var history = chats.ChatHistory.ToList();
                Assert.Multiple(() =>
                {
                    Assert.That(history[i].Value.Name, Is.EqualTo($"Test{i}"));
                    Assert.That(history[i].Value.Message, Is.EqualTo($"A test is being conducted{i}"));
                    Assert.That(history[i].Value.UserID, Is.EqualTo(776 + i));
                });
            }

            Assert.Multiple(() =>
            {
                Assert.That(chats.FirstMsg.Message, Is.EqualTo(chats.ChatHistory[0].Message));
                Assert.That(chats.FirstMsg.Name, Is.EqualTo(chats.ChatHistory[0].Name));
            });
            Assert.Multiple(() =>
            {
                Assert.That(chats.TrimChatHistory(2), Is.EqualTo(3));
                Assert.That(chats.ChatHistory[1].Name, Is.EqualTo("Test3"));
            });
            Console.WriteLine(chats.Chat_TotalTokens(chats.CharacterProfile.ProfileInfo("BobTheTester")));//912 tokens
            var printedHistory = chats.PrintHistory(true);
            Assert.Multiple(() =>
            {
                Assert.That(printedHistory, !Does.Contain("{{char}}"));
                Assert.That(printedHistory, !Does.Contain("{{user}}"));
            });
        }
    }
}
