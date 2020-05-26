using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using YouTubeSearch;
using koichibot.Essentials;

namespace koichibot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        [Command("writeperm")]
        [Summary("readperm")]
        public async Task WritePermAsync(IGuildUser guildUser, string perm, bool allow)
        {
            try
            {
                guildUser.WritePermission(perm, allow);
                await ReplyAsync("done");
                return;
            }
            catch (Exception ex)
            {
                await StaticMethods.ExceptionHandler(ex, Context);
                return;
            }
        }

        [Command("readperm")]
        [Summary("readperm")]
        public async Task ReadPermAsync(IGuildUser guildUser, string perm)
        {
            try
            {
                await ReplyAsync(guildUser.ReadPermission(perm).ToString());
            }
            catch (Exception ex)
            {
                await StaticMethods.ExceptionHandler(ex, Context);
                return;
            }
        }
    }
}
