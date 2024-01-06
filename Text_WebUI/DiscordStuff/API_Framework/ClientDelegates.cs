using Discord.Commands;
using Discord.WebSocket;
using Discord_AI_Presence.Text_WebUI.MemoryManagement;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_AI_Presence.Text_WebUI.DiscordStuff.API_Framework
{
    /// <summary>
    /// Create this class in your class that contains the client, SocketCommandContext and SocketUserMessage and pass the dependencies in the constructor.    
    /// </summary>
    internal class ClientDelegates
    {
        private DiscordSocketClient _client;
        private SocketCommandContext _context;
        private SocketUserMessage _message;
        public ClientDelegates(DiscordSocketClient client, SocketCommandContext context, SocketUserMessage message)
        {
            _client = client;
            _context = context;
            _message = message;
            _client.MessageReceived += MessageReceived;
            _client.MessageDeleted += MessageDeleted;
        }

        /// <summary>
        /// Here we are deleting a message from the message history if it is removed from Discord.
        /// </summary>
        /// <param name="m">The message from the cache</param>
        /// <param name="c">The channel the message is from</param>
        /// <returns></returns>
        private async Task MessageDeleted(Cacheable<IMessage, ulong> m, Cacheable<IMessageChannel, ulong> c)
        {
            if ((await c.GetOrDownloadAsync()) is not SocketTextChannel channel) return;
            var serverData = TextUI_Base.GetInstance().ServerData.FirstOrDefault(x => x.ServerID == channel.Guild.Id);
            if (serverData == null) return;
            // loading after the previous two, why waste a connection check if the other two are going to be null anyways.
            var message = await m.GetOrDownloadAsync();
            var chat = serverData.FindChatByMsgID(message.Id);
            if (chat == null) return;
            chat.RemoveMessage(message.Content);
        }

        private async Task MessageReceived(SocketMessage MessageParam)
        {
            _message = MessageParam as SocketUserMessage;
            if (_message == null)
                return;
            _context = new SocketCommandContext(_client, _message);
            if (_context.User.IsBot)
                return;
            var aiProfile = TextUI_Base.GetInstance().Check(_message.Content);
            if (aiProfile != null)
                return;
            var serverData = TextUI_Base.GetInstance().ServerData.FirstOrDefault(x => x.ServerID == _context.Guild.Id);
            if (serverData != null && DiscordMessageHistory.ShouldEndChat(_message))
            {
                serverData.EndChat(aiProfile.NickOrName());
                return;
            }
            // If a new chat is not started but it detects a current AI chat in progress, it will add to the chat memory instead.
            await TextUI_Base.GetInstance().StartChat(_context, aiProfile.NickOrName());
            if(serverData.ServerSettings.AllowRandomAIOccurance)
            {
                bool shouldReply = Extensions.ReturnRandom(0, 100) > 95 && Extensions.ReturnRandom(0, 100) > 96;
                if (shouldReply)
                {

                }
            }
        }
    }
}
