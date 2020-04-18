using Discord;
using Discord.Commands;
using JikanDotNet;
using koichibot.Essentials;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace koichibot.Modules.Mal
{
    public class MalCommands : ModuleBase<SocketCommandContext>
    {
        [Command("anime")]
        [Summary("Searches up anime from MAL.")]
        public async Task AnimeSearchAsync([Optional] params string[] message)
        {
            try
            {
                if (message is null || message.Length == 0)
                {
                    return;
                }

                string final = StaticMethods.ParseText(message);
                final = final.Remove(0, 1);

                IJikan jikan = new Jikan(true);

                AnimeSearchResult animeSearchResult = await jikan.SearchAnime(final);
                Anime anime = jikan.GetAnime(animeSearchResult.Results.First().MalId).Result;

                string status = "";
                string airing = "";
                if (animeSearchResult.Results.First().Airing)
                {
                    status = "Airing";
                    airing = animeSearchResult.Results.First().StartDate.ToString().Remove(animeSearchResult.Results.First().StartDate.ToString().Length - 9, 9) + " - Still Airing";
                }
                else
                {
                    status = "Finished";
                    airing = animeSearchResult.Results.First().StartDate.ToString().Remove(animeSearchResult.Results.First().StartDate.ToString().Length - 9, 9) +
                        " - " + animeSearchResult.Results.First().EndDate.ToString().Remove(animeSearchResult.Results.First().EndDate.ToString().Length - 9, 9);
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
        [Summary("Searches up manga from MAL.")]
        public async Task MangaSearchAsync([Optional] params string[] message)
        {
            try
            {
                if (message is null || message.Length == 0)
                {
                    return;
                }

                string final = StaticMethods.ParseText(message);
                final = final.Remove(0, 1);

                IJikan jikan = new Jikan(true);

                MangaSearchResult mangaSearchResult = await jikan.SearchManga(final);
                Manga manga = jikan.GetManga(mangaSearchResult.Results.First().MalId).Result;

                string status;
                string publishing;
                if (mangaSearchResult.Results.First().Publishing)
                {
                    status = "Publishing";
                    publishing = mangaSearchResult.Results.First().StartDate.ToString().Remove(mangaSearchResult.Results.First().StartDate.ToString().Length - 9, 9) + " - Still running";
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
    }
}
