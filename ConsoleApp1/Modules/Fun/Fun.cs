using Discord;
using Discord.Commands;
using koichibot.Essentials;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using YouTubeSearch;

namespace koichibot.Modules.Fun
{
    public class Fun : ModuleBase<SocketCommandContext>
    {
        [Command("yt")]
        [Summary("Searches YouTube for your query and return a link to the first result.")]
        public async Task GetYtLink([Optional] params string[] rawQuery)
        {
            try
            {
                if (rawQuery is null || rawQuery.Length == 0)
                {
                    await ReplyAsync("usage: `b!yt <query>`");
                    return;
                }

                string searchQuery = StaticMethods.ParseText(rawQuery);

                VideoSearch videos = new VideoSearch();
                var items = await videos.GetVideos(searchQuery, 1);

                //EmbedBuilder embed = new EmbedBuilder();
                await ReplyAsync(items.First().getUrl());
            }
            catch (InvalidOperationException ex)
            {
                await ReplyAsync($"Such video doesn't exist.\n" +
                    $"{ex.GetType().ToString()}: {ex.Message}");
                return;
            }
        }

        [Command("define")]
        [Summary("Returns the first result from urban dictionary.")]
        public async Task GetDefinitionAsync([Optional] params string[] rawQuery)
        {
            try
            {
                if (rawQuery is null || rawQuery.Length == 0)
                {
                    await ReplyAsync("Usage: b!define <query>");
                    return;
                }
                Methods methods = new Methods();

                string searchQuery = StaticMethods.ParseText(rawQuery).Remove(0, 1);
                var response = methods.GetUrbanQuery(searchQuery).Result;

                EmbedBuilder embedBuilder = new EmbedBuilder();
                embedBuilder.WithTitle(response.Word)
                    .WithDescription(response.Definition)
                    .AddField("Example", response.Example, false)
                    .AddField("Rating", response.ThumbsUp + " :thumbsup: / " + response.ThumbsDown + " :thumbsdown:", true)
                    .AddField("Author", response.Author, true)
                    .WithFooter("Powered by urbandictionary.com")
                    .WithColor(Color.DarkMagenta);

                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            catch (AggregateException)
            {
                // i realize that this catch is pretty sloppy but i can't be bothered really
                await ReplyAsync($"No definition for `{StaticMethods.ParseText(rawQuery).Remove(0, 1)}` exists.");
                return;
            }
            catch (Exception ex)
            {
                await ReplyAsync("something silly happened:\n" + ex.ToString());
                return;
            }
        }
    }
}
