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
            TextUI_Servers ser = new(0000000, new(), []);
            ser.AIChats.Add(0123, new Chats(0543, TextUI_Base.GetInstance().Cards.First().Value, new TextUI_Presets(), "BobTheTester"));
            ser.AIChats.First().Value.AddMessage("Test", "A test is being conducted", 777, 900);
            ser.AIChats.First().Value.AddMessage("Test2", "A test is being conducted2", 778, 901);
            ser.AIChats.First().Value.AddMessage("Test3", "A test is being conducted3", 779, 902);
            ser.AIChats.First().Value.AddMessage("Test4", "A test is being conducted4", 780, 903);
            var first = ser.AIChats.First().Value;
            for (int i = 1; i < ser.AIChats.Values.Count; i++)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(first.ChatHistory[i].Name, Is.EqualTo($"Test{i}"));
                    Assert.That(first.ChatHistory[i].Message, Is.EqualTo($"A test is being conducted{i}"));
                    Assert.That(first.ChatHistory[i].UserID, Is.EqualTo(776 + i));
                    Assert.That(first.ChatHistory[i].MsgID, Is.EqualTo(899 + i));
                });
            }

            Assert.Multiple(() =>
            {
                Assert.That(first.FirstMsg.Message, Is.EqualTo("A test is being conducted"));
                Assert.That(first.FirstMsg.Name, Is.EqualTo("Test"));
                Assert.That(first.Get_TotalWordCount, Is.EqualTo(24));
            });
            Assert.Multiple(() =>
            {
                Assert.That(first.TrimChatHistory(2), Is.EqualTo(2));
                Assert.That(first.ChatHistory[0].Name, Is.EqualTo("Test3"));
            });
            Console.WriteLine(first.Chat_TotalTokens(first.CharacterProfile.ProfileInfo()));//912 tokens
            var printedHistory = first.PrintHistory(true);
            Assert.Multiple(() =>
            {
                Assert.That(printedHistory, !Does.Contain("{{char}}"));
                Assert.That(printedHistory, !Does.Contain("{{user}}"));
            });
        }
    }
}
