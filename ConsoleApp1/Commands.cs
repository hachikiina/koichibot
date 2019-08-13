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
            Anime anime = jikan.GetAnime(animeSearchResult.Results.First().MalId).Result;

            await ReplyAsync(animeSearchResult.Results.First().MalId.ToString() + " " + anime.MalId); //debug purposes

            string bruh;
            string brah;
            if (animeSearchResult.Results.First().Airing)
            {
                bruh = "Airing";
                brah = animeSearchResult.Results.First().StartDate.ToString().Remove(10, 9) + " - Still Airing";
            }
            else
            {
                bruh = "Finished";
                brah = animeSearchResult.Results.First().StartDate.ToString().Remove(10, 9) + 
                    " - " + animeSearchResult.Results.First().EndDate.ToString().Remove(10, 9);
            }

            EmbedBuilder embedBuilder = new EmbedBuilder();


            try
            {
                embedBuilder.WithTitle(anime.Title)
                    .WithThumbnailUrl(anime.ImageURL)
                    .WithDescription(anime.Synopsis)
                    .WithColor(Color.DarkGreen)
                    .AddField("Status", bruh, true)
                    .AddField("Type", anime.Type, true)
                    .AddField("Aired", brah, true)
                    .AddField("Rank", anime.Rank, true);
            }
            catch (System.Exception ex)
            {
                await ReplyAsync(ex.Message);
            }
            

            await ReplyAsync("", false, embedBuilder.Build());
        }

        [Command("manga")]
        public async Task MangaSearch([Optional] params string[] message)
        {
            string final = "";
            foreach (var item in message)
                final = final + " " + item.ToString();

            final = final.Remove(0, 1);

            IJikan jikan = new Jikan(true);

            MangaSearchResult mangaSearchResult = await jikan.SearchManga(final);
            Manga manga = jikan.GetManga(mangaSearchResult.Results.First().MalId).Result;

            string bruh;
            string brah;
            if (mangaSearchResult.Results.First().Publishing)
            {
                bruh = "Publishing";
                brah = mangaSearchResult.Results.First().StartDate.ToString().Remove(10, 9) + " - Still running";
            }
            else
            {
                bruh = "Finished";
                brah = $"{ mangaSearchResult.Results.First().StartDate.ToString().Remove(10, 9) } - { mangaSearchResult.Results.First().EndDate.ToString().Remove(10, 9) }";
            }

            EmbedBuilder embedBuilder = new EmbedBuilder();

            embedBuilder.WithTitle(manga.Title)
                .WithDescription(manga.Synopsis)
                .WithColor(Color.LightOrange)
                .WithThumbnailUrl(manga.ImageURL)
                .AddField("Status", bruh, true)
                .AddField("Type", manga.Type, true)
                .AddField("Published", brah, true)
                .AddField("Rank", manga.Rank, true);

            await ReplyAsync("", false, embedBuilder.Build());
        }
    }
}
