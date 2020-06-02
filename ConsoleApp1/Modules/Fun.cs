using Discord;
using Discord.Commands;
using koichibot.Essentials;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using YouTubeSearch;
using Serilog;
using Discord.WebSocket;

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
                Serilog.Log.Error(ex, "Issue while getting a video:");
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
                    await ReplyAsync("Usage: `b!define <query>`");
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
                    .WithUrl(response.Permalink.AbsoluteUri)
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
                    .WithColor(Context.User.GetGuildUserRoleColor());

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
                await StaticMethods.ExceptionHandler(ex, Context);
                return;
            }
        }

        [Command("flipcoin")]
        [Summary("Flips a coin for you.")]
        public async Task FlipCoinAsync()
        {
            Methods methods = new Methods();
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle("The result is...")
                .WithColor(Context.User.GetGuildUserRoleColor())
                .WithFooter(Context.User.Username + "#" + Context.User.DiscriminatorValue, Context.User.GetAvatarUrl());
            Random random = new Random();
            int result = random.Next(0, 100);
            // i can maybe download these from the links later or smth idk...
            if (result > 50)
            {
                embedBuilder.WithDescription("Tails!");
                embedBuilder.WithImageUrl("https://upload.wikimedia.org/wikipedia/commons/6/64/1TL_obverse.png");
            }
            else
            {
                embedBuilder.WithDescription("Heads!");
                embedBuilder.WithImageUrl("https://upload.wikimedia.org/wikipedia/commons/c/cd/1TL_reverse.png");
            }

            await ReplyAsync("", embed: embedBuilder.Build());
            return;
        }

        [Command("rolldice")]
        [Summary("Rolls a dice.")]
        public async Task RollDiceAsync([Optional] params string[] type)
        {
            try
            {
                if (type.Length == 0)
                {
                    Random random = new Random();
                    Methods methods = new Methods();
                    int rndNum = random.Next(1, 7);

                    EmbedBuilder embedBuilder = new EmbedBuilder();
                    embedBuilder.WithTitle("Your random number is: " + rndNum)
                        .WithColor(Context.Message.Author.GetGuildUserRoleColor())
                        .WithImageUrl("attachment://" + Context.Message.Id.ToString() + ".png");

                    await Context.Channel.SendFileAsync(methods.DrawRegularPolygonSK(6, 500f, Context, rndNum), embed: embedBuilder.Build());
                }
                else
                {
                    bool success = int.TryParse(type.ParseText(), out int maxVal);
                    if (success)
                    {
                        if (maxVal >= 1 && maxVal <= 2)
                        {
                            await FlipCoinAsync();
                            return;
                        }

                        if (maxVal > 0)
                        {
                            Random random = new Random();
                            Methods methods = new Methods();
                            int rndNum = random.Next(1, maxVal + 1);

                            EmbedBuilder embedBuilder = new EmbedBuilder();
                            embedBuilder.WithTitle("Your random number is: " + rndNum)
                                .WithColor(Context.Message.Author.GetGuildUserRoleColor())
                                .WithImageUrl("attachment://" + Context.Message.Id.ToString() + ".png");

                            await Context.Channel.SendFileAsync(methods.DrawRegularPolygonSK(maxVal, 500f, Context, rndNum), embed: embedBuilder.Build());
                            return;
                        }
                        else
                        {
                            await ReplyAsync("Please provide a workable number. It should a positive integer.");
                            return;
                        }
                    }
                    else
                    {
                        await ReplyAsync("Please provide a workable number. It should a positive integer.");
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                await StaticMethods.ExceptionHandler(ex, Context);
                return;
            }
        }
    }
}
