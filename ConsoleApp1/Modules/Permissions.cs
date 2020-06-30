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
    [Group("permission")]
    [Alias("perms", "permissions")]
    public class Permissions : ModuleBase<SocketCommandContext>
    {
        // perms allow node --user user
        [Command("allow")]
        [Summary("Allows the user to use that command.")]
        public async Task AllowAsync(string node, IGuildUser user)
        {
            bool allow = false;
            try
            {
                allow = user.ReadPermission(node);
            }
            catch (System.IO.DirectoryNotFoundException)
            {
                user.WritePermission(node, true);
            }
            catch (System.IO.FileNotFoundException)
            {
                user.WritePermission(node, true);
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
                user.WritePermission(node, true);
            }

            if (allow)
            {
                await ReplyAsync("It's already allowed!");
                return;
            }
            else
            {
                await ReplyAsync($"Set the {node} node of {user.ToString()} to true.");
                return;
            }
        }
    }
}
