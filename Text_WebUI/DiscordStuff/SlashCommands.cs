using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord_AI_Presence.Text_WebUI.MemoryManagement;
using Discord_AI_Presence.Text_WebUI.Presets;
using Microsoft.VisualBasic;
using static Discord_AI_Presence.Text_WebUI.Presets.TextUI_Presets;
namespace Discord_AI_Presence.Text_WebUI.DiscordStuff
{
    public class SlashCommands : InteractionModuleBase<SocketInteractionContext>
    {
        
        [SlashCommand("changepreset", "Change your preset. Only the person who started the chat can change it")]
        public async Task ChangePreset([Summary("Presets", "Choose a preset for your chat")] PresetEnum presets)
        {
            try
            {
                //await Context.Interaction.DeferAsync();
                var curServer = TextUI_Base.GetInstance().ServerData.FirstOrDefault(x => x.ServerID == Context.Guild.Id);
                curServer = TextUI_Base.GetInstance().ServerData.First(x => x.ServerID == Context.Guild.Id);
                var chat = curServer.AIChats.FirstOrDefault(x => x.ChatStarterUserID == Context.Interaction.User.Id);
                if (chat == null)
                {
                    await RespondAsync("You do not have a chat going right now.");
                    return;
                }
                //chat.Presets.ChangePreset(presets);
                await Context.Interaction.RespondAsync($"Preset has been changed to {presets}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [SlashCommand("swapintro", "Swap the intro the character made. This can be swapped at anytime in the conversation.")]
        public async Task SwapIntro()
        {
            var curServer = TextUI_Base.GetInstance().ServerData.First(x => x.ServerID == Context.Guild.Id);
            var chat = curServer.AIChats.FirstOrDefault(x => x.ChatStarterUserID == Context.Interaction.User.Id);
            if (chat == null)
            { 
                await RespondAsync("You do not have a chat going right now.");
                return;
            }
            else if (chat.CharacterProfile.AllGreetings.Count == 1)
            {
                await RespondAsync("There are no other greetings.");
                return;
            }
            var greetingIndex = chat.CharacterProfile.GreetingsIterator(chat.FindGreeting(chat.CharacterProfile));
            if (greetingIndex == -1)
            {
                await RespondAsync("No greeting was found. If this message shows up there's a bug, report it as an issue at https://github.com/HumbleCCoder/Discord_AI_Presence.");
                return;
            }

            var greetings = chat.CharacterProfile.AllGreetings;
            if ((await Context.Guild.GetTextChannel(chat.ChannelID).GetMessageAsync(chat.FirstMsg.MsgID)) is not SocketUserMessage msgToEdit)
            {
                await RespondAsync("No message found. If this message shows up the message might have been deleted. Otherwise report as a bug at https://github.com/HumbleCCoder/Discord_AI_Presence.");
                return;
            }
            await msgToEdit.ModifyAsync(x => x.Content = greetings[greetingIndex]);
        }

        [SlashCommand("startchat", "Begins a new chat with an AI. A user can only be engaged with one AI at a time, per server.")]
        public void StartChat()
        {

            throw new NotImplementedException();
        }
    }
}
