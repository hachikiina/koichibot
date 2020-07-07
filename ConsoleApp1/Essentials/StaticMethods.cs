using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using Serilog;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace koichibot.Essentials
{
    public static class StaticMethods
    {
        public static ulong OwnerID = 309758882425733121;
        public static async Task ExceptionHandler(Exception ex, SocketCommandContext context)
        {
            await context.Channel.SendMessageAsync($"Something went wrong, `{ex.GetType()}`");
            //await context.Channel.SendMessageAsync($"```{ex.ToString()}```");
            Log.Error($"Something went wrong at {context.Guild.Name}/{context.Channel.Name}");
            Log.Error($"Message content: \"{context.Message.Content}\"");
            Log.Error($"({context.Guild.Id}/{context.Channel.Id}/{context.Message.Id})");
            Log.Error(ex, "Details: ");
            Log.Error("----------------------------------------------");
            return;
        }

        public static bool IsFileEmpty(string fileName)
        {
            FileInfo info = new FileInfo(fileName);

            if (info.Length == 0)
                return true;

            if (info.Length < 6)
            {
                string content = File.ReadAllText(fileName);
                return content.Length == 0;
            }

            return false;
        }
    }

}
