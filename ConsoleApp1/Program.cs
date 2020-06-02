using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using koichibot.Essentials;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace koichibot
{
    public class Program
    {
        public DiscordSocketClient Client;
        private CommandService Commands;
        //private InteractiveService Interactive;
        private IServiceProvider Services;
        private SettingsJson Settings = new SettingsJson();

        static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            // logger initializiton
            try
            {
                Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.Console()
                        .WriteTo.Logger(lc => lc.Filter.ByIncludingOnly(e => e.Level == Serilog.Events.LogEventLevel.Error).WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "error-.txt"), rollingInterval: RollingInterval.Day))
                        .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "latest-.txt"), rollingInterval: RollingInterval.Day)
                        .CreateLogger();

                Log.Information("Starting up...");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while initializing logger!");
                Console.WriteLine(ex.ToString());
                return;
            }
            // logger initialization end

            Client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Debug,
            });

            Commands = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = false,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug,
                IgnoreExtraArgs  = true,
                
            });

            //Interactive = new InteractiveService(Client);

            Services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton(Commands)
                .AddSingleton<InteractiveService>()
                .BuildServiceProvider();


            await Commands.AddModulesAsync(Assembly.GetEntryAssembly(), Services);
            Commands.CommandExecuted += OnCommandExecutedAsync;
            Client.MessageReceived += HandleCommandAsync;

            Client.Ready += Client_Ready;
            Client.Log += Client_Log;

            try
            {
                string dirData = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
                if (!Directory.Exists(dirData))
                {
                    Directory.CreateDirectory(dirData);
                }
                if (!File.Exists(Path.Combine(dirData, "settings.json")))
                {
                    // i haven't disposed it because it'll only run once and the program will end afterwards.
                    using (StreamWriter sw = new StreamWriter(File.Create(Path.Combine(dirData, "settings.json"))))
                    {
                        var tempSet = new SettingsJson { Token = "TOKEN_HERE", Prefix = "PREFIX_HERE" };
                        sw.WriteLine(JsonConvert.SerializeObject(tempSet, Formatting.Indented));
                    }
                    Log.Error("There is no settings.json; created one. Please enter your settings in it.");
                    return;
                }

                try
                {
                    using (var stream = new FileStream(Path.Combine(dirData, "settings.json"), FileMode.Open, FileAccess.Read))
                    using (var readFile = new StreamReader(stream))
                        Settings = JsonConvert.DeserializeObject<SettingsJson>(readFile.ReadToEnd().Replace("\n", ""));
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error while reading settings:");
                    return;
                }

                try
                {
                    await Client.LoginAsync(TokenType.Bot, Settings.Token);
                    await Client.StartAsync();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error while starting the bot up:");
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unexpected error:");
                return;
            }

            await Task.Delay(-1);
        }

        private async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!string.IsNullOrEmpty(result?.ErrorReason))
            {
                await context.Channel.SendMessageAsync(result.ErrorReason);
            }
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            SocketUserMessage msg = arg as SocketUserMessage;

            if (msg is null || msg.Author.IsBot) return;

            ContextCheck contextCheck = new ContextCheck();
            await contextCheck.CheckLink(new SocketCommandContext(Client, msg), arg);

            int argPos = 0;
            if (msg.HasStringPrefix(Settings.Prefix, ref argPos) || msg.HasMentionPrefix(Client.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(Client, msg);
                await Commands.ExecuteAsync(context, argPos, Services);
            }
        }

        private Task Client_Log(LogMessage Message)
        {
            Log.Information($"[{Message.Source}] {Message.Message}");
            return Task.CompletedTask;
        }

        private async Task Client_Ready()
        {
            await Client.SetGameAsync(Settings.Prefix + "help | sup?", null, Discord.ActivityType.Playing);
        }
    }
}
