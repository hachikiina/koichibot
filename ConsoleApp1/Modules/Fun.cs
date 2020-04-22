using Discord;
using Discord.Commands;
using koichibot.Essentials;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using YouTubeSearch;

namespace koichibot.Modules
{
    public class Fun : ModuleBase<SocketCommandContext>
    {
        [Command("yt")]
        [Summary("Searches YouTube for your query and returns a link to the first result.")]
        public async Task GetYtLink([Optional] params string[] rawQuery)
        {
            try
            {
                if (rawQuery is null || rawQuery.Length == 0)
                {
                    await ReplyAsync("usage: `b!yt <query>`");
                    return;
                }

                string searchQuery = rawQuery.ParseText();

                VideoSearch videos = new VideoSearch();
                var items = await videos.GetVideos(searchQuery, 1);

                //EmbedBuilder embed = new EmbedBuilder();
                await ReplyAsync(items.First().getUrl());
            }
            catch (InvalidOperationException ex)
            {
                await ReplyAsync($"Such video doesn't exist.");
                return;
            }
        }

        [Command("define")]
        [Summary("Returns the first result from Urban Dictionary.")]
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

                string searchQuery = rawQuery.ParseText();
                var response = methods.GetUrbanQuery(searchQuery).Result;

                if (response.Definition.Length > 2000)
                {
                    await ReplyAsync("The definitions character count is over the limits of what Discord accepts.\n" +
                        "Here's a link to the definition: " + $"<{response.Permalink}>");
                    return;
                }

                EmbedBuilder embedBuilder = new EmbedBuilder();
                embedBuilder.WithTitle(response.Word)
                    .WithDescription(response.Definition);

                if (response.Example != "")
                {
                    if (response.Example.Length > 1024)
                    {
                        embedBuilder.AddField("Example", response.Example.Remove(1018) + "[...]", false);
                    }
                    else
                    {
                        embedBuilder.AddField("Example", response.Example, false);
                    }
                }

                embedBuilder.AddField("Rating", response.ThumbsUp + " :thumbsup: / " + response.ThumbsDown + " :thumbsdown:", true)
                    .AddField("Author", response.Author, true)
                    .WithFooter("Powered by urbandictionary.com")
                    .WithColor(Color.DarkMagenta);

                await ReplyAsync("", false, embedBuilder.Build());
                return;
            }
            catch (AggregateException)
            {
                // i realize that this catch is pretty sloppy but i can't be bothered really
                await ReplyAsync($"No definition for `{rawQuery.ParseText()}` exists.");
                return;
            }
            catch (Exception ex)
            {
                await StaticMethods.ExceptionHandler(ex, Context.Channel);
                return;
            }
        }
    }
}
