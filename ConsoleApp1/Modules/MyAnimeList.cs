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
using System.Timers;

namespace koichibot.Modules
{
    public class MyAnimeList : InteractiveBase<SocketCommandContext>
    {
        [Command("anime")]
        [Summary("Searches up anime from MAL.")]
        public async Task AnimeSearchAsync([Optional] params string[] animeName)
        {
            if (animeName.Length == 0)
            {
                await ReplyAsync("Kullanım: `b!anime <anime>`");
                return;
            }

            try
            {
                var prompt = await ReplyAsync($"`{Format.Sanitize(animeName.ParseText())}` için sonuçlar alınıyor...");
                StringBuilder strBuilder = new StringBuilder();
                IJikan jikan = new Jikan(true);

                AnimeSearchResult animeSearch = await jikan.SearchAnime(animeName.ParseText());
                Dictionary<int, AnimeSearchEntry> searchResults = new Dictionary<int, AnimeSearchEntry>();
                int i = 0;
                strBuilder.Append($"[Results for {Format.Sanitize(animeName.ParseText())}]")
                    .Append(Environment.NewLine);
                foreach (var anime in animeSearch.Results)
                {
                    if (i == 10)
                    {
                        strBuilder.Append(Environment.NewLine)
                            .Append("Çıkan sonuçların solundaki numarayı yazman yeterli. Örn -> \"3\"");
                        break;
                    }
                    strBuilder.Append((i + 1) + " - " + anime.Title)
                        .Append(Environment.NewLine);
                    searchResults.Add(i + 1, anime);
                    i++;
                }

                await prompt.ModifyAsync(msg => msg.Content = Format.Code(strBuilder.ToString(), "css"));

                var timespan = TimeSpan.FromSeconds(10);
                while (timespan > TimeSpan.Zero)
                {
                    if (timespan.TotalSeconds < TimeSpan.FromSeconds(1).TotalSeconds) break;

                    var response = await NextMessageAsync(timeout: TimeSpan.FromMilliseconds(200));
                    if (response != null)
                    {
                        if (int.TryParse(response.Content, out int intAnime) && intAnime <= 10 && intAnime > 0)
                        {
                            var anime = jikan.GetAnime(searchResults.GetValueOrDefault(intAnime).MalId).Result;
                            MalMethods mal = new MalMethods();
                            await prompt.ModifyAsync(msg =>
                            { 
                                msg.Content = "";
                                msg.Embed = mal.CreateAnimeEmbed(anime, Context.User);
                            });
                            await response.DeleteAsync();
                            return;
                        }
                    }
                    timespan = timespan.Subtract(TimeSpan.FromMilliseconds(200));
                    await Task.Delay(200);
                }
                await prompt.DeleteAsync();
                await ReplyAsync("Zaman aşımı.");

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
                var prompt = await ReplyAsync($"`{Format.Sanitize(mangaName.ParseText())}` için sonuçlar alınıyor...");
                StringBuilder strBuilder = new StringBuilder();
                IJikan jikan = new Jikan(true);

                var mangaSearch = jikan.SearchManga(mangaName.ParseText()).Result;
                Dictionary<int, MangaSearchEntry> searchResults = new Dictionary<int, MangaSearchEntry>();
                int i = 0;
                strBuilder.Append($"[Results for {Format.Sanitize(mangaName.ParseText())}]")
                    .Append(Environment.NewLine);
                foreach (var manga in mangaSearch.Results)
                {
                    if (i == 10)
                    {
                        strBuilder.Append(Environment.NewLine)
                            .Append("Çıkan sonuçların solundaki numarayı yazman yeterli. Örn -> \"3\"");
                        break;
                    }
                    strBuilder.Append((i + 1) + " - " + manga.Title)
                        .Append(Environment.NewLine);
                    searchResults.Add(i + 1, manga);
                    i++;
                }

                await prompt.ModifyAsync(msg => msg.Content = Format.Code(strBuilder.ToString(), "css"));

                var timespan = TimeSpan.FromSeconds(10);
                while (timespan > TimeSpan.Zero)
                {
                    if (timespan.TotalSeconds < TimeSpan.FromSeconds(1).TotalSeconds) break;

                    var response = await NextMessageAsync(timeout: TimeSpan.FromMilliseconds(200));
                    if (response != null)
                    {
                        if (int.TryParse(response.Content, out int intManga) && intManga <= 10 && intManga > 0)
                        {
                            // embed builder to show it bae.
                            var manga = jikan.GetManga(searchResults.GetValueOrDefault(intManga).MalId).Result;
                            MalMethods mal = new MalMethods();
                            await prompt.ModifyAsync(msg =>
                            {
                                msg.Content = "";
                                msg.Embed = mal.CreateMangaEmbed(manga, Context.User);
                            });
                            await response.DeleteAsync();
                            return;
                        }
                    }
                timespan = timespan.Subtract(TimeSpan.FromMilliseconds(200));
                await Task.Delay(200);
                }

                await prompt.DeleteAsync();
                await ReplyAsync("Zaman aşımı.");

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
