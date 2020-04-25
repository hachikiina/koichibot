using Discord;
using Discord.Commands;
using Discord.WebSocket;
using koichibot.Essentials;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Serilog;

namespace koichibot
{
    public class Program : ModuleBase<SocketCommandContext>
    {
        public DiscordSocketClient Client;
        private CommandService Commands;
        private IServiceProvider Services;

        static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            // logger initializiton
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.Logger(lc => lc.Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Error).WriteTo.File("logs\\error-.txt", rollingInterval: RollingInterval.Day))
                .WriteTo.File("logs\\latest-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Starting up...");
            // logger initialization end

            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug,
            });

            Commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug,
            });

            Services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(Commands)
                .BuildServiceProvider();

            Client.MessageReceived += HandleCommandAsync;
            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            Client.Ready += Client_Ready;
            Client.Log += Client_Log;

            string Token = "";
            using (var Stream = new FileStream((Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).Replace(@"bin\Debug\netcoreapp2.0", @"data\token.txt"), FileMode.Open, FileAccess.Read))
            using (var ReadToken = new StreamReader(Stream))
            {
                Token = ReadToken.ReadToEnd();
            }
            await Client.LoginAsync(Discord.TokenType.Bot, Token);
            await Client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;
            string compare = msg.ToString().ToLower();

            if (msg is null || msg.Author.IsBot) return;

            int argPos = 0;
            if (msg.HasStringPrefix("b!", ref argPos) || msg.HasMentionPrefix(Client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(Client, msg);

                await Commands.ExecuteAsync(context, argPos, Services);
            }
        }

        private async Task Client_Log(LogMessage Message)
        {
            Log.Information($"[{Message.Source}] {Message.Message}");
        }

        private async Task Client_Ready()
        {
            await Client.SetGameAsync("b!help | sup?", null, Discord.ActivityType.Playing);
        }
    }
}
