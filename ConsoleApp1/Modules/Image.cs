using Discord;
using Discord.Commands;
using Discord.WebSocket;
using koichibot.Essentials;
using System.Threading.Tasks;

namespace koichibot.Modules
{
    public class Image : ModuleBase<SocketCommandContext>
    {
        [Command("ahegao")]
        [Summary("Sends ahegao, that's all.")]
        public async Task AhegaoAsync()
        {
            Methods methods = new Methods();

            EmbedBuilder embed = new EmbedBuilder();

            embed.WithTitle("Here's your random ahegao")
                .WithImageUrl(await methods.GetAhegaoFromEg())
                .WithFooter("powered by egecelikci's ahegao stash")
                .WithColor(Color.DarkerGrey);

            await ReplyAsync("", false, embed.Build());
            return;
        }

        [Command("thighs")]
        [Summary("Sends thighs, that's all. Oh and it's mostly NSFW! ~~I thought the name gave that away...~~")]
        public async Task ThighsAsync()
        {
            SocketTextChannel channel = Context.Channel as SocketTextChannel;
            if (!channel.IsNsfw)
            {
                await ReplyAsync("This channel is not an nsfw channel, halting.");
                return;
            }
            Methods methods = new Methods();
            string url = methods.GetUrlFromNeko();
            if (url != null)
            {
                EmbedBuilder embedBuilder = new EmbedBuilder();
                embedBuilder.WithTitle("Here's your thigh pic :^)")
                    .WithImageUrl(url)
                    .WithFooter("Powered by nekobot.xyz");

                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            else
            {
                await ReplyAsync("failed to get link from nekobot");
                return;
            }
        }
    }
}
