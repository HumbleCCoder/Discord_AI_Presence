using Discord_AI_Presence.Text_WebUI;
using Discord_AI_Presence.Text_WebUI.Presets;
using Discord.Net;
using Discord.WebSocket;
using Discord;
using Discord.Commands;
using System.Diagnostics.CodeAnalysis;
using Discord.Interactions;
using System.Reflection;
namespace Discord_AI_Presence
{
    /// <summary>
    /// This is a rough draft for slash command testing purposes since they can't be done through unit testing here.
    /// Proper code writing will happen later when I'm near completion of the backend things.
    /// </summary>
    internal class Program
    {
        [NotNull]
        public static DiscordSocketClient Client = new();
        public static InteractionService InteractionServices { get; private set; }
        private CommandService _commands = new();
        static async Task Main()
            => await new Program().MainAsync();

        private async Task MainAsync()
        {
            _commands = new(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = Discord.Commands.RunMode.Async,
                LogLevel = LogSeverity.Critical,
            });
            Client = new(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true,
                MessageCacheSize = 250,
                LogGatewayIntentWarnings = false
            });
            Client.Log += message => // this one
            {
                Console.WriteLine(message);
                return Task.CompletedTask;
            };
            Client.Ready += Client_Ready;
            var token = File.ReadAllText(@$"{Environment.CurrentDirectory}\BotKeys.txt");
            Client.LoginAsync(TokenType.Bot, token).GetAwaiter().GetResult();
            await Client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task Client_Ready()
        {
            InteractionServiceConfig t = new()
            {
                UseCompiledLambda = true,
                DefaultRunMode = Discord.Interactions.RunMode.Async
            };
            InteractionServices = new InteractionService(Client, t);
            var serverID = File.ReadAllText(@$"{Environment.CurrentDirectory}\HelperFile.txt");
            await InteractionServices.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            await InteractionServices.RegisterCommandsToGuildAsync(ulong.Parse(serverID), false);
            Client.InteractionCreated -= async interaction =>
            {
                var ctx = new SocketInteractionContext(Client, interaction);
                await InteractionServices.ExecuteCommandAsync(ctx, null);
            };
            Client.InteractionCreated += async interaction =>
            {
                var ctx = new SocketInteractionContext(Client, interaction);
                await InteractionServices.ExecuteCommandAsync(ctx, null);
            };
        }
    }
}