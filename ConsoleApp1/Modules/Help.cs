using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

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

        [
            Command("help"),
            Summary("Lists commands with their explanation")
        ]
        public async Task HelpAsync(string query)
        {
            StringBuilder strBuilder = new StringBuilder();
            //string footerText;
            if (query is null)
            {
                foreach (var item in _commands.Modules.Where(m => m.Parent is null))
                {
                    strBuilder.Append(item.Name);
                    strBuilder.Append(Environment.NewLine);
                }
                //footerText = 
            }
            else
            {
                await ReplyAsync("todo");
            }

            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.WithDescription(strBuilder.ToString())
                .WithTitle("Help");
            await ReplyAsync("", false, embedBuilder.Build());
            return;
        }
    }
}
