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
using System.Collections.Generic;

namespace koichibot.Essentials
{
    public class ContextCheck
    {
        private static readonly Regex MessageRegex = new Regex("(https://(?:(?:canary|ptb).)?(?:discord|discordapp).com/channels/(\\d+)/(\\d+)/(\\d+))+", RegexOptions.Singleline | RegexOptions.Compiled);
        public async Task CheckLink(SocketCommandContext context, SocketMessage arg)
        {
            string messageContent = arg.Content;
            List<string> matches = new List<string>();
            foreach (var input in messageContent.Split(' '))
            {
                if (MessageRegex.IsMatch(input))
                {
                    matches.Add(input);
                }
            }

            if (matches.Count() > 0) 
            {
                Methods methods = new Methods();
                await methods.QuoteAsync(context, matches.First(), messageContent.Split(' ').ToList());
            }
            return;
        }

        
    }
}
