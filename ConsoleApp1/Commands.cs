using Discord;
using Discord.Commands;
using Discord.WebSocket;
using JikanDotNet;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Net;
using YouTubeSearch;

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
                .WithDescription("```css" +
                "\n`b!help`\t\tBu mesajı gösterir." +
                "\n`b!bruh`\t\tbruh" +
                "\n`b!avatar`\t\tAvatar url atar" +
                "\n`b!ping`\t\tpong!" +
                "\n`b!say`\t\tyazdığını yazar" +
                "\n`b!sayd`\t\tyazdığını yazıp orijinal mesajı siler." +
                "\n`b!sendmessage`\t\tbelirtilen kanalda mesaj gönderir" +
                "\n`b!anime`\t\tanime arat" +
                "\n`b!manga`\t\tmanga arat" +
                "\n`b!x`\t\tdoubt" +
                "\n```")
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

                await ReplyAsync(final);
                //await Context.Guild.GetTextChannel(617113668550787260).SendMessageAsync($"{ Context.Message.Author.Username }#{ Context.Message.Author.Discriminator }: { Context.Message }");

            }
        }

        [Command("sayd")]
        public async Task SayDAsync([Optional] params string[] message)
        {
            if (message.Length == 0)
            {
                await ReplyAsync("Usage: `b!sayd <message>`");
            }
            else
            {
                string final = "";
                foreach (var item in message)
                    final = final + " " + item.ToString();

                await ReplyAsync(final);
                await Context.Message.DeleteAsync();
                //await Context.Guild.GetTextChannel(617113668550787260).SendMessageAsync($"{ Context.Message.Author.Username }#{ Context.Message.Author.Discriminator }: { Context.Message }");
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
                    await Context.Guild.GetTextChannel(617113668550787260).SendMessageAsync($"{ Context.Message.Author.Username }#{ Context.Message.Author.Discriminator }: { Context.Message }");

                }
                else
                {
                    await ReplyAsync("Please enter a valid channel.");
                }

            }
        }

        [Command("anime")]
        public async Task AnimeSearchAsync([Optional] params string[] message)
        {
            try
            {
                if (message is null || message.Length == 0)
                {
                    return;
                }

                string final = "";
                foreach (var item in message)
                    final = final + " " + item.ToString();

                final = final.Remove(0, 1);

                IJikan jikan = new Jikan(true);

                AnimeSearchResult animeSearchResult = await jikan.SearchAnime(final);
                Anime anime = jikan.GetAnime(animeSearchResult.Results.First().MalId).Result;

                string bruh = "";
                string brah = "";
                if (animeSearchResult.Results.First().Airing)
                {
                    bruh = "Airing";
                    brah = animeSearchResult.Results.First().StartDate.ToString().Remove(animeSearchResult.Results.First().StartDate.ToString().Length - 9, 9) + " - Still Airing";
                }
                else
                {
                    bruh = "Finished";
                    brah = animeSearchResult.Results.First().StartDate.ToString().Remove(animeSearchResult.Results.First().StartDate.ToString().Length - 9, 9) +
                        " - " + animeSearchResult.Results.First().EndDate.ToString().Remove(animeSearchResult.Results.First().EndDate.ToString().Length - 9, 9);
                }

                EmbedBuilder embedBuilder = new EmbedBuilder();

                embedBuilder.WithTitle(anime.Title)
                    .WithThumbnailUrl(anime.ImageURL)
                    .WithDescription(anime.Synopsis)
                    .WithColor(Color.DarkGreen)
                    .AddField("Status", bruh, true)
                    .AddField("Type", anime.Type, true)
                    .AddField("Aired", brah, true)
                    .AddField("Rank", anime.Rank, true);

                await ReplyAsync("", false, embedBuilder.Build());
            }
            catch (System.ArgumentOutOfRangeException ex)
            {
                //await ReplyAsync("Böyle bir anime yok.");
                //await ReplyAsync($"Detaylar: \n```css" +
                //    $"\nStack trace\t\t:\t\t{ ex.StackTrace } " +
                //    $"\nInner exception\t\t:\t\t{ ex.InnerException }" +
                //    $"```");

                if (ex.InnerException != null)
                {
                    await ReplyAsync("Böyle bir anime yok.");
                    await ReplyAsync($"Detaylar: \n```css" +
                        $"\nStack trace\t\t:\t\t{ ex.StackTrace } " +
                        $"\nInner exception\t\t:\t\t{ ex.InnerException }" +
                        $"```");
                }
                else
                {
                    await ReplyAsync("Böyle bir anime yok.");
                    await ReplyAsync($"Detaylar: \n```css" +
                        $"\nStack trace\t\t:\t\t{ ex.StackTrace } " +
                        $"```");
                }
            }
            catch (System.InvalidOperationException)
            {
                await ReplyAsync("botu bozmaya çalışma amk");
            }
            catch (System.Exception ex)
            {
                await ReplyAsync($"Beklenmedik bir hata gerçekleşti <:floshed:605840584376188948>:\n```css" +
                    $"\nHata kodu\t:\t{ ex.GetType() }\n" +
                    $"Hata mesajı\t:\t{ ex.Message }\n" +
                    $"Stack Trace\t:\t{ ex.StackTrace }```");
            }
        }

        [Command("manga")]
        public async Task MangaSearchAsync([Optional] params string[] message)
        {
            try
            {
                if (message is null || message.Length == 0)
                {
                    return;
                }

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
                    brah = mangaSearchResult.Results.First().StartDate.ToString().Remove(mangaSearchResult.Results.First().StartDate.ToString().Length - 9, 9) + " - Still running";
                }
                else
                {
                    bruh = "Finished";
                    brah = $"{ mangaSearchResult.Results.First().StartDate.ToString().Remove(mangaSearchResult.Results.First().StartDate.ToString().Length - 9, 9) } - " +
                        $"{ mangaSearchResult.Results.First().EndDate.ToString().Remove(mangaSearchResult.Results.First().EndDate.ToString().Length - 9, 9) }";
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
            catch (System.ArgumentOutOfRangeException)
            {
                await ReplyAsync("Böyle bir manga yok.");
            }
            catch (System.InvalidOperationException)
            {
                await ReplyAsync("botu bozmaya çalışma amk");
            }
            catch (System.Exception ex)
            {
                await ReplyAsync($"Beklenmedik bir hata gerçekleşti <:floshed:605840584376188948>:\n```css" +
                    $"\nHata kodu: " +
                    $"\n  { ex.GetType()}" +
                    $"\n\nHata mesajı: " +
                    $"\n    { ex.Message }" +
                    $"```");
            }
        }

        //[Command ("character")]
        //public async Task CharacterSearchAsync([Optional] params string[] message)
        //{
        //    try
        //    {
        //        if (message is null || message.Length == 0)
        //        {
        //            return;
        //        }

        //        string final = "";
        //        foreach (var item in message)
        //            final = final + " " + item.ToString();

        //        final = final.Remove(0, 1);

        //        IJikan jikan = new Jikan(true);

        //        CharacterSearchResult characterSearchResult = await jikan.SearchCharacter(final);
        //        Character character = jikan.GetCharacter(characterSearchResult.Results.First().MalId).Result;

        //        EmbedBuilder builder = new EmbedBuilder();

        //        builder.WithTitle(character.Name)
        //            .WithDescription(character.About)
        //            .WithThumbnailUrl(character.ImageURL);

        //        await ReplyAsync("", false, builder.Build());

        //    }
        //    catch (System.ArgumentOutOfRangeException)
        //    {
        //        await ReplyAsync("Böyle bir karakter yok.");
        //    }
        //    catch (System.InvalidOperationException)
        //    {
        //        await ReplyAsync("botu bozmaya çalışma amk");
        //    }
        //    catch (System.Exception ex)
        //    {
        //        await ReplyAsync($"Beklenmedik bir hata gerçekleşti <:floshed:605840584376188948>:\n```css" +
        //            $"\nHata kodu: " +
        //            $"\n  { ex.GetType()}" +
        //            $"\n\nHata mesajı: " +
        //            $"\n    { ex.Message }" +
        //            $"```");

        //        System.Console.WriteLine($"Error =>\n { ex.ToString() }\n\n stack trace =>\n{ ex.StackTrace }");
        //    }
        //}

        [Command("x")]
        public async Task DoubtAsync()
        {
            await ReplyAsync("The one, who used `b!x`, surely has some doubts.");
        }
        #region kardo
        //[Command("trymanga")]
        //public async Task Mangatrial([Optional] params string[] message)
        //{
        //    try
        //    {
        //        if (message is null || message.Length == 0)
        //        {
        //            return;
        //        }

        //        string final = "";
        //        foreach (var item in message)
        //            final = final + " " + item.ToString();

        //        final = final.Remove(0, 1);

        //        IJikan jikan = new Jikan(true);

        //        string lol = "The first 5 results are:\n```css\n";
        //        MangaSearchResult mangaSearchResult = await jikan.SearchManga(final);
        //        int i = 0;
        //        foreach (var item in mangaSearchResult.Results)
        //        {
        //            lol = lol + $"{ i + 1 } - " + item.Title + "\n";
        //            if (i == 4)
        //            {
        //                lol = lol + "```";
        //                await ReplyAsync(lol);
        //                return;
        //            }
        //            i++;
        //        }
        //        string input;
        //        bool done = false;
        //        while (!done)
        //        {
        //            //await for the users message about 1-5 and select wiselyyyyyyyyyyyyyyyyyyyyy
        //        }
        //        #region bruh
        //        //Manga manga = jikan.GetManga(mangaSearchResult.Results.First().MalId).Result;

        //        //string bruh;
        //        //string brah;
        //        //if (mangaSearchResult.Results.First().Publishing)
        //        //{
        //        //    bruh = "Publishing";
        //        //    brah = mangaSearchResult.Results.First().StartDate.ToString().Remove(10, 9) + " - Still running";
        //        //}
        //        //else
        //        //{
        //        //    bruh = "Finished";
        //        //    brah = $"{ mangaSearchResult.Results.First().StartDate.ToString().Remove(10, 9) } - { mangaSearchResult.Results.First().EndDate.ToString().Remove(10, 9) }";
        //        //}

        //        //EmbedBuilder embedBuilder = new EmbedBuilder();

        //        //embedBuilder.WithTitle(manga.Title)
        //        //    .WithDescription(manga.Synopsis)
        //        //    .WithColor(Color.LightOrange)
        //        //    .WithThumbnailUrl(manga.ImageURL)
        //        //    .AddField("Status", bruh, true)
        //        //    .AddField("Type", manga.Type, true)
        //        //    .AddField("Published", brah, true)
        //        //    .AddField("Rank", manga.Rank, true);

        //        //await ReplyAsync("", false, embedBuilder.Build());
        //        #endregion
        //    }
        //    catch (System.ArgumentOutOfRangeException)
        //    {
        //        await ReplyAsync("Böyle bir manga yok.");
        //    }
        //    catch (System.InvalidOperationException)
        //    {
        //        await ReplyAsync("botu bozmaya çalışma amk");
        //    }
        //    catch (System.Exception ex)
        //    {
        //        await ReplyAsync($"Beklenmedik bir hata gerçekleşti <:floshed:605840584376188948>:\n```css" +
        //            $"\nHata kodu: " +
        //            $"\n  { ex.GetType()}" +
        //            $"\n\nHata mesajı: " +
        //            $"\n    { ex.Message }" +
        //            $"```");
        //    }
        //}
        #endregion

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("ban")]
        public async Task BanUAsync([Optional] IGuildUser user, [Optional] params string[] reason)
        {
            if (user is null)
            {
                await ReplyAsync("Kullanım: `b!ban <kullanıcı> [sebep]`");
                return;
            }

            string final = "";
            if (reason.Length != 0 || !(reason is null))
            {
                foreach (var item in reason)
                    final = final + " " + item.ToString();
            }
            else
                final = "Sebep belirtilmedi.";

            await Context.Guild.AddBanAsync(user, 0, final);
        }

        [Command("yt")]
        public async Task GetYtLink([Optional] params string[] rawQuery)
        {
            if (rawQuery is null || rawQuery.Length == 0)
            {
                await ReplyAsync("usage: `b!yt <query>`");
                return;
            }

            string searchQuery = "";
            foreach (var item in rawQuery)
                searchQuery = searchQuery + " " + item.ToString();

            VideoSearch videos = new VideoSearch();
            var items = await videos.GetVideos(searchQuery, 1);

            //EmbedBuilder embed = new EmbedBuilder();
            await ReplyAsync(items.First().getUrl());
        }

        [RequireUserPermission(GuildPermission.Administrator)]
        [Command("summon")]
        public async Task Summon([Optional] IGuildUser user)
        {
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

        [Command("ahegao")]
        public async Task AhegaoAsync()
        {
            Methods methods = new Methods();
            string url = await methods.GetRandomImageFromEg();

            EmbedBuilder embed = new EmbedBuilder();

            embed.WithTitle("Here's your random ahegao")
                .WithImageUrl(await methods.GetRandomImageFromEg())
                .WithFooter("powered by egecelikci's ahegao stash")
                .WithColor(Color.DarkerGrey);

            await ReplyAsync("", false, embed.Build());
        }

        [Command("thighs")]
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
            }
            else
            {
                await ReplyAsync("failed to get link from nekobot");
            }
        }
    }
}
