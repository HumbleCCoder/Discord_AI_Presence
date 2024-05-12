using Discord_AI_Presence.Text_WebUI;
using Discord_AI_Presence.Text_WebUI.DiscordStuff;
using Discord_AI_Presence.Text_WebUI.MemoryManagement;
using Discord_AI_Presence.Text_WebUI.TextWebUI;
using Discord_AI_Presence.Text_WebUI.ProfileScripts;
using Discord_AI_Presence.Text_WebUI.Presets;
using Discord_AI_Presence.Text_WebUI.Instructions;
namespace Discord_AI_Presence.UnitTesting
{
    internal class NeuroNetTesting
    {
        private const string username = "Jeff",
                message = "Hello how are you?";
        private const ulong
            serverId = 5,
            channelId = 9,
            chatStarterUserID = 55,
            msgID = 99;
        private readonly ProfileData characterProfile = TextUI_Base.GetInstance().Cards.First().Value.First();
        [Test]
        public async Task SubmitToNeuroNet()
        {
            var server = new TextUI_Servers(serverId);
            ChatParameters cm = new(characterProfile.NickOrName(), message);
            server.StartChat(new Chats(channelId, characterProfile, username, Text_WebUI.Instructions.Scenario.ScenarioPresets.Chatbot, 12345, cm, (int)chatStarterUserID));
            server.AIChats.First().Value.AddMessage(username, message, chatStarterUserID, msgID);
            var result = await Connection.PostMessage(characterProfile, server, server.AIChats.First().Value, server.AIChats.First().Value.Participants());
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
            Console.WriteLine(result);
        }
    }
}
