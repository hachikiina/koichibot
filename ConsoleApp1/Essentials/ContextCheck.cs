using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using System.Text.RegularExpressions;
using System.Linq;

namespace koichibot.Essentials
{
    public class ContextCheck
    {
        private static readonly Regex MessageRegex = new Regex("(https://(?:(?:canary|ptb).)?(?:discord|discordapp).com/channels/(\\d+)/(\\d+)/(\\d+))+", RegexOptions.Singleline | RegexOptions.Compiled);
        public async Task CheckLink(SocketCommandContext context, SocketMessage arg)
        {
            string messageContent = arg.Content;
            var matches = MessageRegex.Matches(messageContent.Split(' ').First());

            if (matches.Count > 0) 
            {
                Methods methods = new Methods();
                foreach (var match in matches)
                {
                    await methods.QuoteAsync(context, messageContent);
                }
            }
            return;
        }

        
    }
}
