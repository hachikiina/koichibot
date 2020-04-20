using Discord;
using Discord.Commands;
using koichibot.Essentials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace koichibot.Modules
{
    [Summary("Contains the help command.")]
    public class Help : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        public Help(IServiceProvider services, CommandService commands)
        {
            _commands = commands;
            _services = services;
        }

        [Command("help")]
        [Summary("Lists commands with their explanation")]
        public async Task HelpAsync([Optional] params string[] query)
        {
            try
            {
                List<EmbedFieldBuilder> fieldBuilders = new List<EmbedFieldBuilder>();
                string footerText;
                if (query.Length == 0)
                {
                    footerText = "Use `b!help [moduleName]` to get an in-depth prompt.";
                    foreach (var item in _commands.Modules.Where(m => m.Parent is null))
                    {
                        if (item.Commands.Count == 0) continue;

                        StringBuilder commandsBuilder = new StringBuilder();
                        EmbedFieldBuilder fieldBuilder = new EmbedFieldBuilder();
                        fieldBuilder.WithIsInline(false)
                            .WithName(item.Name);

                        foreach (var cmd in item.Commands)
                        {
                            commandsBuilder.Append($"`{cmd.Name}`, ");
                        }
                        fieldBuilder.WithValue(commandsBuilder.ToString().Remove(commandsBuilder.Length - 2));
                        fieldBuilders.Add(fieldBuilder);
                    }

                    EmbedBuilder embedBuilder = new EmbedBuilder();
                    embedBuilder.WithDescription("Here are the commands:")
                        .WithTitle("Help")
                        .WithFields(fieldBuilders)
                        .WithFooter(footerText)
                        .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())
                        .WithColor(Color.DarkOrange);
                    await ReplyAsync("", false, embedBuilder.Build());
                    //await ReplyAsync(strBuilder.ToString());
                    return;
                }
                else
                {
                    EmbedBuilder embedBuilder = new EmbedBuilder();

                    string moduleName = query.ParseTextExt().Trim();
                    bool success = false;

                    foreach (var item in _commands.Modules.Where(m => m.Parent is null))
                    {
                        if (moduleName.ToLower().Replace("ı", "i") != item.Name.ToLower().Replace("ı", "i")) continue;
                        embedBuilder.WithTitle(item.Name + " Commands");
                        foreach (var cmd in item.Commands)
                        {
                            EmbedFieldBuilder fieldBuilder = new EmbedFieldBuilder();
                            StringBuilder usageSummBuilder = new StringBuilder();

                            usageSummBuilder.Append(cmd.Summary + Environment.NewLine)
                                .Append("**Usage:** ");
                            string temp = "";
                            foreach (var arg in cmd.Parameters)
                            {
                                temp = temp + $"<{arg.Name}>" + " ";
                            }
                            usageSummBuilder.Append($"`{ (cmd.Name + " " + temp).TrimEnd() }`" + Environment.NewLine);

                            fieldBuilder.WithIsInline(false)
                                .WithName(cmd.Name)
                                .WithValue(usageSummBuilder.ToString());
                            fieldBuilders.Add(fieldBuilder);
                        }
                        success = true;
                    }
                    if (!success)
                    {
                        await ReplyAsync("Such module doesn't exist.");
                        return;
                    }

                    embedBuilder.WithFields(fieldBuilders)
                    .WithColor(Color.DarkOrange);

                    await ReplyAsync("", false, embedBuilder.Build());
                    //await ReplyAsync(usageSummBuilder.ToString());
                    return;
                }
            }
            catch (Exception ex)
            {
                await StaticMethods.ExceptionHandler(ex, Context.Channel);
                return;
            }
        }
    }
}
