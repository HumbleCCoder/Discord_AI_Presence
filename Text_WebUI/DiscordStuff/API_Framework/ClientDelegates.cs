﻿using Discord.Commands;
using Discord.WebSocket;
using Discord_AI_Presence.Text_WebUI.MemoryManagement;
using Discord_AI_Presence.Text_WebUI.DiscordStuff.API_Framework;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Channels;
using Discord_AI_Presence.Text_WebUI.ProfileScripts;
using Discord.Rest;
using Discord_AI_Presence.Text_WebUI.Button_Related;
using Discord_AI_Presence.DebugThings;
using Discord_AI_Presence.Text_WebUI.Instructions;

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

        public ClientDelegates(ref DiscordSocketClient client, ref SocketCommandContext context, ref SocketUserMessage message)
        {
            _client = client;
            _context = context;
            _message = message;
            // Client populates itself every time it reconnects to the API server. We want to make sure these delegates only exist once.
            _client.MessageReceived -= MessageReceived; 
            _client.MessageDeleted -= MessageDeleted;
            _client.ButtonExecuted -= ButtonHandler;
            _client.MessageReceived += MessageReceived;
            _client.MessageDeleted += MessageDeleted;
            _client.ButtonExecuted += ButtonHandler;
        }

        /// <summary>
        /// This is Discord's delegate for handling button interactions. When a button is clicked, it sends the component with the labels/custom ID and styles
        /// for us to use.
        /// </summary>
        /// <param name="component">The Discord interaction component</param>
        /// <returns></returns>
        private async Task ButtonHandler(SocketMessageComponent component)
        {
            // Components use a nullable type so it must be cast to a normal ulong.
            var theGuild = TextUI_Base.GetInstance().ServerData[(ulong)component.GuildId];
            var buttonHandler = theGuild.DuplicateHandling[component.Channel.Id];
            // This allow the interaction to continue without failing when a reply does not need to be sent or the reply should be sent later.
            await component.DeferAsync(); 
            switch (component.Data.CustomId)
            {
                case ButtonHandling.NextID:
                    await buttonHandler.SwapMessage();
                    break;
                case ButtonHandling.OKID:
                    await buttonHandler.ChooseCharacter(component);
                    break;
            }
        }

        /// <summary>
        /// Here we are deleting a message from the message history if it is removed from Discord.
        /// </summary>
        /// <param name="m">The message from the cache</param>
        /// <param name="c">The channel the message is from</param>
        /// <returns></returns>
        private async Task MessageDeleted(Cacheable<IMessage, ulong> m, Cacheable<IMessageChannel, ulong> c)
        {
            if ((await c.DownloadAsync()) is not SocketTextChannel channel)
                return;
            if (!TextUI_Base.GetInstance().ServerData.TryGetValue(channel.Guild.Id, out var serverData))
                return;
            var message = m.Id;
            var msg = serverData.FindMsgByMsgID(message, channel.Id);
            if (msg.Equals(default(Memory)))
                return;
            var chat = serverData.FindChat(channel.Id);
            chat.RemoveMessage(message);
        }

        /*
         * To do 1/12/2024: 
         * Handle random appearances of AI.
         * Cleanup the duplicate detection code.
         * Comb over the order of operations.
         * Add way for a custom scenario to be added via text like hey (name) Scenario: "".
         * Double check the flow of scenario information to make sure it's entering memory and posting to chat properly.
           Because chatbots should not open with their character introduction. The instruction for chatbot should be entered into memory
           but not sent to the server chat. This will help tell the AI that they are chatting online with people.
         * Lastly, I need to make a better way to get AI messages submitted to memory. It's a recipe for disaster to allow
         * webhooks to make it into the MessageReceived delegate.
         */
        private async Task MessageReceived(SocketMessage MessageParam)
        {
            _message = MessageParam as SocketUserMessage;
            if (_message == null || _message.Author.IsBot)
                return;

            _context = new SocketCommandContext(_client, _message);
            var instance = TextUI_Base.GetInstance();
            var serverData = instance.ServerData[_context.Guild.Id];

            if (!serverData.AIChats.TryGetValue(_message.Channel.Id, out var chats) && serverData.ServerSettings.AllowRandomAIOccurance)
            {
                // WIP random occurance is a random AI character popping out without anyone calling it. We'll handle that here.
                return;
            }
            var aiProfile = instance.Check(_message.Content, chats != null);

            // Automatically end a chat if it's time to go.
            // If it didn't find any AI in the check, disregard
            if (serverData.EndChat(_context.Channel.Id, DiscordMessageHistory.ShouldEndChat(_message)) || aiProfile == null)
                return;

            // Send an AI reply if the conditions are met. Chats should not be null here.
            if (await chats.aiFlow.SendAiChat(_message.Content, _context))
                return;

            /* Multiple characters with the same name is handled here.
             * It will generate a "next" and "submit" button to let the user cycle through characters
             * and view them by their profile picture then click submit to choose.
             */
            if (aiProfile.Count > 1)
            {
                if (serverData.DuplicateHandling.ContainsKey(_message.Channel.Id))
                    return;

                var starterId = _message.Author.Id;
                var botMsg = await _context.Channel.SendMessageAsync("There are multiple characters. Please pick one.");
                bool serverChannelType = serverData.ServerSettings.DedicatedAIChannels.Exists(x => x == _context.Channel.Id);

                // Only dedicated channels can be a roleplay or custom scenario, otherwise always pick chatbot.
                var scenarioType = serverChannelType ? Scenario.ScenarioPresets.Roleplay : Scenario.ScenarioPresets.Chatbot;
                serverData.DuplicateHandling.Add(_context.Channel.Id, new Button_Related.ButtonHandling(aiProfile, botMsg, starterId, scenarioType));
                await serverData.DuplicateHandling.Last().Value.SwapMessage();
                return;
            }
            // If a new chat is not started but it detects a current AI chat in progress, it will add to the chat memory instead.
            await TextUI_Base.GetInstance().StartChat(_context, aiProfile[0].NickOrName(), 0);
        }
    }
}
