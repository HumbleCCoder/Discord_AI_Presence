using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Discord;
using System.Reflection;

namespace Discord_AI_Presence.Text_WebUI.DiscordStuff.API_Framework
{
    public class Client
    {
        private DiscordSocketClient ClientObj = new();
        private InteractionService InteractionServices { get; set; }
        private CommandService _commands = new();
        private SocketCommandContext Context;// { get; set; }
        private SocketUserMessage Message;// { get; set; }
        private ClientDelegates Delegates;//
        private static Client _instance;
        private static object _instanceLock = new ();

        public static Client GetInstance()
        {
            if (_instance == null)
            {
                lock (_instanceLock)
                {
                    _instance ??= new Client();
                }
            }
            return _instance;
        }

        public SocketGuild FindGuild(ulong guildId) => GetInstance().ClientObj.GetGuild(guildId);

        public string FindUsername(ulong userID)
        {
            var user = GetInstance().ClientObj.GetUser(userID);
            return user.GlobalName ?? user.Username;
        }

        private static async Task Main()
        { 
            await GetInstance().MainAsync();            
        }

        private async Task MainAsync()
        {
            _commands = new(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = Discord.Commands.RunMode.Async,
                LogLevel = LogSeverity.Critical,
            });
            ClientObj = new(new DiscordSocketConfig
            {
                GatewayIntents = GatewayIntents.All,
                AlwaysDownloadUsers = true,
                MessageCacheSize = 250,
                LogGatewayIntentWarnings = false
            });
            ClientObj.Log += message => // this one
            {
                Console.WriteLine(message);
                return Task.CompletedTask;
            };
            ClientObj.Ready += Client_Ready;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), null); 
            var token = File.ReadAllText(@$"{Environment.CurrentDirectory}\BotKeys.txt");
            ClientObj.LoginAsync(TokenType.Bot, token).GetAwaiter().GetResult();
            await ClientObj.StartAsync();

            await Task.Delay(-1);
        }

        private async Task HandleInteraction(SocketInteraction interaction)
        {
            var ctx = new SocketInteractionContext(ClientObj, interaction);
            await InteractionServices.ExecuteCommandAsync(ctx, null);
        }

        private async Task Client_Ready()
        {
            InteractionServiceConfig t = new()
            {
                UseCompiledLambda = true,
                DefaultRunMode = Discord.Interactions.RunMode.Async
            };
            Delegates = new(ref ClientObj, ref Context, ref Message);
            InteractionServices = new InteractionService(ClientObj, t);
            var serverID = File.ReadAllText(@$"{Environment.CurrentDirectory}\HelperFile.txt");
            await InteractionServices.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            await InteractionServices.RegisterCommandsToGuildAsync(ulong.Parse(serverID), false);

            ClientObj.InteractionCreated -= HandleInteraction;
            ClientObj.InteractionCreated += HandleInteraction;
            TextUI_Base.GetInstance().PopulateServerData([.. ClientObj.Guilds]);
        }
    }
}
