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
                MalMethods malMethods = new MalMethods();
                var prompt = await ReplyAsync($"`{Format.Sanitize(animeName.ParseText())}` için sonuçlar alınıyor...");
                IJikan jikan = new Jikan(true);

                string animeList = await malMethods.GenerateAnimeListAsync(animeName.ParseText())
                    + Environment.NewLine + "Çıkan sonuçların solundaki numarayı yazman yeterli. Örn -> \"3\""
                    +Environment.NewLine + "İptal etmek için de \"c\" yazman yeterli.";

                await prompt.ModifyAsync(msg => msg.Content = Format.Code(animeList, "css"));

                // TODO this whole timespan thing doesn't work, gotta fix it
                // well it does work but i need something better lol
                // idea: actually get the datetime.now and return after its 10 seconds past ?? nice.
                var timespan = TimeSpan.FromSeconds(10);
                while (timespan > TimeSpan.Zero)
                {
                    if (timespan.TotalSeconds < TimeSpan.FromSeconds(1).TotalSeconds) break;

                    var response = await NextMessageAsync(timeout: TimeSpan.FromMilliseconds(200));
                    if (response != null)
                    {
                        // this is all finnicky
                        if (int.TryParse(response.Content, out int intAnime) && intAnime <= 10 && intAnime > 0)
                        {
                            var animes = await malMethods.GetAnimeListAsync(animeName.ParseText());
                            var anime = await jikan.GetAnime(animes.GetValueOrDefault(intAnime).MalId);
                            await prompt.ModifyAsync(msg =>
                            { 
                                msg.Content = "";
                                msg.Embed = malMethods.CreateAnimeEmbed(anime, Context.User);
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
                IJikan jikan = new Jikan(true);
                MalMethods malMethods = new MalMethods();

                string mangaList = await malMethods.GenerateMangaListAsync(mangaName.ParseText())
                    + Environment.NewLine + "Çıkan sonuçların solundaki numarayı yazman yeterli. Örn -> \"3\""
                    + Environment.NewLine + "İptal etmek için de \"c\" yazman yeterli.";

                await prompt.ModifyAsync(msg => msg.Content = Format.Code(mangaList, "css"));

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
                            var mangas = await malMethods.GetMangaListAsync(mangaName.ParseText());
                            var manga = jikan.GetManga(mangas.GetValueOrDefault(intManga).MalId).Result;
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
