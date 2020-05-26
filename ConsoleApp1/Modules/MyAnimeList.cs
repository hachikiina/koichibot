using Discord;
using Discord.Commands;
using Discord.Addons.Interactive;
using JikanDotNet;
using koichibot.Essentials;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace koichibot.Modules
{
    public class MyAnimeList : InteractiveBase<SocketCommandContext>
    {
        [Command("anime")]
        [Summary("Searches up anime from MAL.")]
        public async Task AnimeSearchAsync([Optional] params string[] query)
        {
            if (query.Length == 0)
            {
                await ReplyAsync("Kullanım: `b!anime <anime>`");
                return;
            }

            try
            {
                StringBuilder strBuilder = new StringBuilder();
                IJikan jikan = new Jikan(true);

                AnimeSearchResult animeSearch = await jikan.SearchAnime(query.ParseText());
                Dictionary<int, AnimeSearchEntry> searchResults = new Dictionary<int, AnimeSearchEntry>();
                int i = 0;
                foreach (var anime in animeSearch.Results)
                {
                    if (i == 10) break;
                    strBuilder.Append((i + 1) + " - " + anime.Title)
                        .Append(Environment.NewLine);
                    searchResults.Add(i + 1, anime);
                    i++;
                }

                var prompt = await ReplyAsync($"```{strBuilder.ToString()}```" + Environment.NewLine +
                    "Yukarıdan seç ve sadece numarayı yaz. Örneğin `3`");

                var timespan = TimeSpan.FromSeconds(10);
                while (timespan != TimeSpan.Zero)
                {
                    if (timespan.TotalSeconds < TimeSpan.FromSeconds(1).TotalSeconds) break;

                    var response = await NextMessageAsync();
                    if (response != null)
                    {
                        if (int.TryParse(response.Content, out int intAnime) && intAnime <= 10 && intAnime > 0)
                        {
                            // embed builder to show it bae.
                            var anime = jikan.GetAnime(searchResults.GetValueOrDefault(intAnime).MalId).Result;
                            Methods methods = new Methods();
                            await prompt.ModifyAsync(msg =>
                            { 
                                msg.Content = "";
                                msg.Embed = methods.CreateAnimeEmbed(anime, Context.User);
                            });
                            await response.DeleteAsync();
                            return;
                        }
                        else
                        {
                            await ReplyAsync("Zaman aşımı.");
                            return;
                        }
                    }
                }
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
        public async Task MangaSearchAsync([Optional] params string[] mangaName)
        {
            if (mangaName.Length == 0)
            {
                await ReplyAsync("Kullanım: `b!manga <manga>`");
                return;
            }

            try
            {
                StringBuilder strBuilder = new StringBuilder();
                IJikan jikan = new Jikan(true);

                var mangaSearch = jikan.SearchManga(mangaName.ParseText()).Result;
                Dictionary<int, MangaSearchEntry> searchResults = new Dictionary<int, MangaSearchEntry>();
                int i = 0;
                foreach (var manga in mangaSearch.Results)
                {
                    if (i == 10) break;
                    strBuilder.Append((i + 1) + " - " + manga.Title)
                        .Append(Environment.NewLine);
                    searchResults.Add(i + 1, manga);
                    i++;
                }

                var prompt = await ReplyAsync($"```{strBuilder.ToString()}```" + Environment.NewLine +
                        "Yukarıdan seç ve sadece numarayı yaz. Örneğin `3`");

                var timespan = TimeSpan.FromSeconds(10);
                while (timespan != TimeSpan.Zero)
                {
                    if (timespan.TotalSeconds < TimeSpan.FromSeconds(1).TotalSeconds) break;

                    var response = await NextMessageAsync();
                    if (response != null)
                    {
                        if (int.TryParse(response.Content, out int intManga) && intManga <= 10 && intManga > 0)
                        {
                            // embed builder to show it bae.
                            var manga = jikan.GetManga(searchResults.GetValueOrDefault(intManga).MalId).Result;
                            Methods methods = new Methods();
                            await prompt.ModifyAsync(msg =>
                            {
                                msg.Content = "";
                                msg.Embed = methods.CreateMangaEmbed(manga, Context.User);
                            });
                            await response.DeleteAsync();
                            return;
                        }
                        else
                        {
                            await ReplyAsync("Zaman aşımı.");
                            return;
                        }
                    }
                }
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
