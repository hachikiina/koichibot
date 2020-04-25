using Discord;
using Discord.Commands;
using JikanDotNet;
using koichibot.Essentials;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace koichibot.Modules
{
    public class MyAnimeList : ModuleBase<SocketCommandContext>
    {
        [Command("anime")]
        [Summary("Searches up anime from MAL.")]
        public async Task AnimeSearchAsync([Optional] params string[] message)
        {
            try
            {
                if (message is null || message.Length == 0)
                {
                    await ReplyAsync("Please enter something to search for.");
                    return;
                }

                string final = message.ParseText();

                IJikan jikan = new Jikan(true);

                AnimeSearchResult animeSearchResult = await jikan.SearchAnime(final);
                Anime anime = jikan.GetAnime(animeSearchResult.Results.First().MalId).Result;

                string status = "";
                string airing = "";
                if (animeSearchResult.Results.First().Airing)
                {
                    status = "Airing";
                    airing = animeSearchResult.Results.First().StartDate
                        .ToString().Remove(animeSearchResult.Results.First().StartDate.ToString().Length - 9, 9) + " - Still Airing";
                }
                else
                {
                    status = "Finished";
                    airing = animeSearchResult.Results.First().StartDate
                        .ToString().Remove(animeSearchResult.Results.First().StartDate.ToString().Length - 9, 9) +
                        " - " + animeSearchResult.Results.First().EndDate
                        .ToString().Remove(animeSearchResult.Results.First().EndDate.ToString().Length - 9, 9);
                }

                EmbedBuilder embedBuilder = new EmbedBuilder();

                embedBuilder.WithTitle(anime.Title)
                    .WithThumbnailUrl(anime.ImageURL)
                    .WithDescription(anime.Synopsis)
                    .WithColor(Color.DarkGreen)
                    .AddField("Status", status, true)
                    .AddField("Type", anime.Type, true)
                    .AddField("Aired", airing, true)
                    .AddField("Rank", anime.Rank, true);

                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                //await ReplyAsync("Böyle bir anime yok.");
                //await ReplyAsync($"Detaylar: \n```css" +
                //    $"\nStack trace\t\t:\t\t{ ex.StackTrace } " +
                //    $"\nInner exception\t\t:\t\t{ ex.InnerException }" +
                //    $"```");

                if (ex.InnerException != null)
                {
                    await ReplyAsync("Böyle bir anime yok.");
                    return;
                    
                }
                else
                {
                    await ReplyAsync("Böyle bir anime yok.");
                    return;
                }
            }
            catch (InvalidOperationException)
            {
                await ReplyAsync("botu bozmaya çalışma amk");
                return;
            }
            catch (Exception ex)
            {
                await StaticMethods.ExceptionHandler(ex, Context);
                return;
            }
        }

        [Command("manga")]
        [Summary("Searches up manga from MAL.")]
        public async Task MangaSearchAsync([Optional] params string[] message)
        {
            try
            {
                if (message is null || message.Length == 0) return;

                string final = message.ParseText();

                IJikan jikan = new Jikan(true);

                MangaSearchResult mangaSearchResult = await jikan.SearchManga(final);
                Manga manga = jikan.GetManga(mangaSearchResult.Results.First().MalId).Result;

                string status;
                string publishing;
                if (mangaSearchResult.Results.First().Publishing)
                {
                    status = "Publishing";
                    publishing = mangaSearchResult.Results.First().StartDate
                        .ToString().Remove(mangaSearchResult.Results.First().StartDate.ToString().Length - 9, 9) + " - Still running";
                }
                else
                {
                    status = "Finished";
                    publishing = $"{ mangaSearchResult.Results.First().StartDate.ToString().Remove(mangaSearchResult.Results.First().StartDate.ToString().Length - 9, 9) } - " +
                        $"{ mangaSearchResult.Results.First().EndDate.ToString().Remove(mangaSearchResult.Results.First().EndDate.ToString().Length - 9, 9) }";
                }

                EmbedBuilder embedBuilder = new EmbedBuilder();

                embedBuilder.WithTitle(manga.Title)
                    .WithDescription(manga.Synopsis)
                    .WithColor(Color.LightOrange)
                    .WithThumbnailUrl(manga.ImageURL)
                    .AddField("Status", status, true)
                    .AddField("Type", manga.Type, true)
                    .AddField("Published", publishing, true)
                    .AddField("Rank", manga.Rank, true);

                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            catch (ArgumentOutOfRangeException)
            {
                await ReplyAsync("Böyle bir manga yok.");
                return;
            }
            catch (InvalidOperationException)
            {
                await ReplyAsync("botu bozmaya çalışma amk");
                return;
            }
            catch (Exception ex)
            {
                await StaticMethods.ExceptionHandler(ex, Context);
                return;
            }
        }

        // todo make it promptable
        [Command("animedev")]
        [Summary("dev tool")]
        public async Task AnimeLookupAsync([Optional] params string[] query)
        {
            try
            {
                StringBuilder strBuilder = new StringBuilder();
                string final = query.ParseText();

                IJikan jikan = new Jikan(true);

                AnimeSearchResult animeSearch = await jikan.SearchAnime(query.ParseText());
                int i = 0;
                foreach (var item in animeSearch.Results)
                {
                    if (i == 10) break;
                    strBuilder.Append((i + 1).ToString() + " - " + item.Title)
                        .Append(Environment.NewLine);
                    i++;
                }

                await ReplyAsync($"```{strBuilder.ToString()}```");
                return;
            }
            catch (Exception ex)
            {
                await StaticMethods.ExceptionHandler(ex, Context);
                return;
            }
        }
    }
}
