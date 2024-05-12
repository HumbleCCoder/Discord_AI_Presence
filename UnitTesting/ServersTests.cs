using Discord_AI_Presence.Text_WebUI;
using Discord_AI_Presence.Text_WebUI.Presets;
using Discord_AI_Presence.Text_WebUI.DiscordStuff;
using Discord_AI_Presence.Text_WebUI.Instructions;
using Discord_AI_Presence.Text_WebUI.MemoryManagement;
using Microsoft.VisualBasic;
using Discord_AI_Presence.Text_WebUI.ProfileScripts;

namespace Discord_AI_Presence.UnitTesting
{
    internal class ServersTests
    {
        private const string username = "Jeff",
                message = "Hello how are you?";
        private const ulong
            serverId = 5,
            channelId = 9,
            chatStarterUserID = 55,
            msgID = 99;
        private readonly ProfileData characterProfile = TextUI_Base.GetInstance().Cards.First().Value.First();
        TextUI_Servers g = new(serverId);
        private Chats Begin()
        {
            ChatParameters cm = new(characterProfile.NickOrName(), message);
            g.StartChat(new Chats(channelId, characterProfile, username, Scenario.ScenarioPresets.Chatbot, 0, cm, (int)chatStarterUserID));
            return g.AIChats.First().Value;
        }

        [Test]
        public void MsgDeleteTesting()
        {
            var chat = Begin();
            chat.AddMessage(username, message, chatStarterUserID, msgID);    
            // First we check if the message is found correctly
            var msg = chat.FindMessageByID(msgID);
            Assert.That(msg.Message, Is.Not.Null);
            Assert.That(msg.Message, Is.Not.Empty);
            Assert.That(msg.Message, Is.EqualTo(message));
            // Now we grab a bool checking if the struct returns its default state (meaning not found) when the message is removed.
            bool check = msg.Equals(default(Memory));
            Assert.That(check, Is.False);
            chat.RemoveMessage(message);
            msg = chat.FindMessageByID(msgID);
            check = msg.Equals(default(Memory));
            Assert.That(check, Is.True);
        }

        [Test]
        public void EndChatTest()
        {
            var chat = Begin();
            chat.AddMessage(username, message, chatStarterUserID, msgID);
            Assert.That(g.AIChats.Count, Is.EqualTo(1)); // chat exists
            g.EndChat(chat.ChannelID, true);
            Assert.That(g.AIChats.Count, Is.EqualTo(0)); // chat no longer exists
        }
    }
}
