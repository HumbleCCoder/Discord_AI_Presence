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
                
                var curServer = TextUI_Base.GetInstance().ServerData[Context.Guild.Id];
                if (curServer.AIChats.TryGetValue(Context.Channel.Id, out var chats))
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

        /*[SlashCommand("startchat", "Begins a new chat with an AI. A user can only be engaged with one AI at a time, per server. Only one AI per channel too.")]
        public async Task StartChat(string characterName, PresetEnum presets, Scenario.ScenarioPresets scenarioType)
        {
            Will work on this much later if it's still needed. Since the dropdown list is limited to 25 choices I don't think this is particularly useful 
            except for bots that are limited to slash commands only. This will be a do when needed thing.
            throw new NotImplementedException();
        }*/
    }
}
