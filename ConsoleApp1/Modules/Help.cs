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
                            commandsBuilder.Append($"`{cmd.Name}` - " + cmd.Summary)
                                .Append(Environment.NewLine);
                        }
                        fieldBuilder.WithValue(commandsBuilder.ToString());
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
                    StringBuilder usageSummBuilder = new StringBuilder();
                    EmbedBuilder embedBuilder = new EmbedBuilder();

                    string moduleName = query.ParseTextExt().Trim();
                    bool success = false;

                    foreach (var item in _commands.Modules.Where(m => m.Parent is null))
                    {
                        if (moduleName.ToLower() != item.Name.ToLower()) continue;
                        embedBuilder.WithTitle(item.Name + " Commands");
                        foreach (var cmd in item.Commands)
                        {
                            EmbedFieldBuilder fieldBuilder = new EmbedFieldBuilder();

                            // todo this is still in progress, trying to find a way to spit out its parameter names.
                            usageSummBuilder.Append(cmd.Summary)
                                .Append(Environment.NewLine)
                                .Append("Usage: " + $"b!{cmd.Name} {cmd.Parameters}")
                                .Append(Environment.NewLine);

                            fieldBuilder.WithIsInline(true)
                                .WithName(cmd.Name)
                                .WithValue(cmd.Summary);
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
