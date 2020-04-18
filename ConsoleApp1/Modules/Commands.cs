using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using YouTubeSearch;

namespace koichibot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        //[Command("helpold")]
        //public async Task HelpAsyncOld()
        //{
        //    EmbedBuilder builder = new EmbedBuilder();

        //    builder.WithTitle("Help lol")
        //        .WithDescription("`ping` \t\t Pong!" +
        //        "\n`avatar` \t\t Gets the users avatar." +
        //        "\n`help` \t\t Displays this message." +
        //        "\n`say` \t\t Replies with the message you wrote." +
        //        "\n`sayd` \t\t Does the same with the `say` but deletes your command afterwards." +
        //        "\n`sendmessage` \t\t Sends a message in a specified text channel." +
        //        "\n`anime` \t\t Fetches the anime you want from MAL and gets the first result." +
        //        "\n`manga` \t\t Fetches the manga you want from MAL and gets the first result." +
        //        "\n`ban` \t\t Bans the specified user, requires administrator privileges." +
        //        "\n`yt` \t\t Gets the URL of the video with the matching title you searched for." +
        //        "\n`summon` \t\t Summons the specified user, only usable by the bot owner." +
        //        "\n`ahegao` \t\t Self explanatory :^)" +
        //        "\n`thighs` \t\t Self Explanatory :^)")
        //        .WithColor(Color.Teal);

        //    await ReplyAsync("", false, builder.Build());
        //}
    }
}
