using Discord;
using Discord.Commands;
using koichibot.Essentials;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace koichibot.Modules
{
    public class Miscellaneous : ModuleBase<SocketCommandContext>
    {
        [Command("x")]
        [Summary("Use it if you have doubts about anything.")]
        public async Task DoubtAsync()
        {
            await ReplyAsync("The one, who used `b!x`, surely has some doubts.");
            return;
        }

        [Command("summon")]
        [Summary("Summon the person you want!")]
        public async Task SummonAsync([Optional] IGuildUser user)
        {
            try
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
                await ReplyAsync("Couldn't find the user.");
                return;
            }
            catch (System.Exception ex)
            {
                await StaticMethods.ExceptionHandler(ex, Context);
                return;
            }
        }

        [Command("setplaying")]
        [Summary("Sets the playing status, owner only.")]
        public async Task SetPlayingAsync([Optional] params string[] message)
        {
            try
            {
                if (Context.User.Id != StaticMethods.OwnerID) return;
                if (message.Length == 0) return;
                await Context.Client.SetGameAsync(message.ParseText(), null, ActivityType.Playing);
                await ReplyAsync("Set the playing status to: " + message.ParseText());
                return;
            }
            catch (System.Exception ex)
            {
                await StaticMethods.ExceptionHandler(ex, Context);
                return; 
            }
        }

        [Command("throw")]
        [Summary("dev")]
        public async Task ThrowAsync()
        {
            try
            {
                throw new System.Exception("shit");
            }
            catch (System.Exception ex)
            {
                await StaticMethods.ExceptionHandler(ex, Context);
                return;
            }
        }
    }
}
