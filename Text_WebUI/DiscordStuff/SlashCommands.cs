using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Discord_AI_Presence.Text_WebUI.Instructions;
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
                if (curServer == null)
                    return;
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

        [SlashCommand("startchat", "Begins a new chat with an AI. A user can only be engaged with one AI at a time, per server. Only one AI per channel too.")]
        public async Task StartChat(string characterName, PresetEnum presets, Scenario.ScenarioPresets scenarioType)
        {
            var curServer = TextUI_Base.GetInstance().ServerData.FirstOrDefault(x => x.ServerID == Context.Guild.Id);
            var chat = new Chats(Context.Channel.Id, presets, characterName, Scenario.GetScenario(scenarioType, characterName), Context.User.Id);
            curServer.StartChat(chat);
            await Context.Interaction.DeferAsync(true);
            await Context.Interaction.DeleteOriginalResponseAsync();

        }
    }
}
