using Discord;
using Discord.Commands;
using Discord.WebSocket;
using koichibot.Essentials;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace koichibot.Modules.Moderation
{
    public class Moderation : ModuleBase<SocketCommandContext>
    {
        [Command("ban")]
        [Summary("Bans the specified user.")]
        public async Task BanUAsync([Optional] IGuildUser user, [Optional] params string[] reason)
        {
            try
            {
                SocketGuildUser guildUser = Context.Message.Author as SocketGuildUser;
                if (!guildUser.GuildPermissions.BanMembers || !guildUser.GuildPermissions.Administrator)
                {
                    await ReplyAsync("You have to be either an administrator or have the permission `BanMembers` to use this command.");
                    return;
                }
                if (user is null)
                {
                    await ReplyAsync("Kullanım: `b!ban <kullanıcı> [sebep]`");
                    return;
                }

                string final;
                if (reason.Length != 0 || !(reason is null))
                {
                    final = StaticMethods.ParseText(reason);
                }
                else
                    final = "Sebep belirtilmedi.";

                await Context.Guild.AddBanAsync(user, 0, final);
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.ToString());
                return;
            }
        }
    }
}
