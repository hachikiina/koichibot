using Discord;
using Discord.Commands;
using JikanDotNet;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace koichibot
{
    public class Commands : ModuleBase<SocketCommandContext>
    {


        [Command("ping")]
        public async Task PingAsync()
        {
            await ReplyAsync("Pong!");
        }

        [Command("bruh")]
        public async Task BruhAsync()
        {
            await ReplyAsync("kardo anı");
        }

        [Command("avatar")]
        public async Task AvatarAsync([Optional] IGuildUser userName)
        {
            if (!(userName is null))
            {
                string avatarUrl = userName.GetAvatarUrl();
                avatarUrl = avatarUrl.Replace("?size=128", "?size=1024");

                EmbedBuilder builder = new EmbedBuilder();

                builder.WithTitle($"{ userName.Username }#{ userName.Discriminator }'s Avatar")
                    .WithImageUrl(avatarUrl)
                    .WithColor(Color.DarkPurple);

                await ReplyAsync("", false, builder.Build());
            }
            else
            {
                string avatarUrl = Context.User.GetAvatarUrl();
                avatarUrl = avatarUrl.Replace("?size=128", "?size=1024");

                EmbedBuilder builder = new EmbedBuilder();

                builder.WithTitle($"{ Context.User.Username }#{ Context.User.Discriminator }'s Avatar")
                    .WithImageUrl(avatarUrl)
                    .WithColor(Color.DarkPurple);

                await ReplyAsync("", false, builder.Build());
            }
        }

        [Command("help")]
        public async Task HelpAsync()
        {
            EmbedBuilder builder = new EmbedBuilder();

            builder.WithTitle("Yardım 😎")
                .WithDescription("`b!help`  Bu mesajı gösterir." +
                "\n`b!bruh`  bruh" +
                "\n`b!avatar` Avatar url atar" +
                "\n`b!ping`  pong!" +
                "\n`b!say`  yazdığını yazar" +
                "\n`b!sayd`  yazdığını yazıp orijinal mesajı siler.")
                .WithColor(Color.Teal);

            await ReplyAsync("", false, builder.Build());
        }

        [Command("say")]
        public async Task SayAsync([Optional] params string[] message)
        {
            if (message.Length == 0)
            {
                await ReplyAsync("Kullanım: `say mesaj`");
            }
            else
            {
                string final = "";
                foreach (var item in message)
                    final = final + " " + item.ToString();

                if (final.Contains("aga") || final.Contains("aqa") || final.Contains("a g a") || final.Contains("a q a"))
                    await ReplyAsync("a kelimesi yazmıyorum pardon");
                else
                    await ReplyAsync(final);
            }
        }

        [Command("sayd")]
        public async Task SayDAsync([Optional] params string[] message)
        {
            if (message.Length == 0)
            {
                await ReplyAsync("Kullanım: `sayd mesaj`");
            }
            else
            {
                string final = "";
                foreach (var item in message)
                    final = final + " " + item.ToString();

                if (final.Contains("aga") || final.Contains("aqa") || final.Contains("a g a") || final.Contains("a q a"))
                    await ReplyAsync("a kelimesi yazmıyorum pardon");
                else
                {
                    await ReplyAsync(final);
                    await Context.Message.DeleteAsync();
                }
            }
        }

        [Command("sendmessage")]
        public async Task SendingMessageAsync([Optional] IGuildChannel channel, [Optional] params string[] message)
        {
            if (channel is null || message.Length == 0)
            {
                await ReplyAsync("Usage: `b!sendmessage <Channel> <message>`");
                return;
            }

            //if (!(channel is IGuildChannel))
            //{
            //    await ReplyAsync("Please enter a valid channel.");
            //    return;
            //}

            string final = "";
            foreach (var item in message)
            {
                final = final + " " + item.ToString();
            }
            if (final.Contains("aga") || final.Contains("aqa") || final.Contains("a g a") || final.Contains("a q a"))
                await ReplyAsync("a kelimesi yazmıyorum pardon");
            else
            {
                string lol = channel.Id.ToString();
                bool success = ulong.TryParse(lol, out ulong finalchannel);
                if (success)
                {
                    await Context.Guild.GetTextChannel(finalchannel).SendMessageAsync(final);
                }
                else
                {
                    await ReplyAsync("Please enter a valid channel.");
                }

            }
        }

        [Command("anime")]
        public async Task AnimeSearch([Optional] params string[] message)
        {
            string final = "";
            foreach (var item in message)
                final = final + " " + item.ToString();

            final = final.Remove(0, 1);

            IJikan jikan = new Jikan(true);

            AnimeSearchResult animeSearchResult = await jikan.SearchAnime(final);

            //string lol = "";
            //foreach (var item in animeSearchResult.Results)
            //{
            //    if (item.Title.ToLower().Contains(final.ToLower()))
            //        lol = lol + " \n" + item.Title;
            //}
            //await ReplyAsync(lol);

            EmbedBuilder embedBuilder = new EmbedBuilder();

            embedBuilder.WithTitle($"{ animeSearchResult.Results.First().Title }")
                .WithThumbnailUrl(animeSearchResult.Results.First().ImageURL)
                .WithDescription(animeSearchResult.Results.First().Description)
                .AddField("Status", animeSearchResult.Results.First().Airing)
                .AddField("Type", animeSearchResult.Results.First().Type)
                .WithColor(Color.DarkGreen);

            await ReplyAsync("", false, embedBuilder.Build());
        }
    }
}
