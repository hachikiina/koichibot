using Discord;
using Discord.Commands;
using koichibot.Essentials;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace koichibot.Modules.Miscellaneous
{
    public class Miscellaneous : ModuleBase<SocketCommandContext>
    {
        [Command("x")]
        [Summary("Use it if you're doubting.")]
        public async Task DoubtAsync()
        {
            await ReplyAsync("The one, who used `b!x`, surely has some doubts.");
        }

        [Command("summon")]
        [Summary("Summon the person you want!")]
        public async Task SummonAsync([Optional] IGuildUser user)
        {
            if (Context.User.Id != StaticMethods.OwnerID)
            {
                await ReplyAsync("You have to be the owner of the bot to use this command (as of now, at least).");
                return;
            }

            var userList = Context.Guild.Users;

            foreach (var item in userList)
            {
                if (item == user)
                {
                    await ReplyAsync("I summon thou, " + item.Mention);
                    return;
                }
            }
        }
    }
}
