using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using YouTubeSearch;
using koichibot.Essentials;

namespace koichibot.Modules
{
    [Group("spoiler")]
    [Summary("Does spoiler stuff i guess")]
    public class Spoiler : ModuleBase<SocketCommandContext>
    {
        //[Command]
        //public async Task SpoilerAsync()
        //{
        //    await ReplyAsync("sup");
        //}

        [Group("create")]
        [RequireOwner(ErrorMessage = "you are not the owner lol")]
        public class Create : ModuleBase<SocketCommandContext>
        {
            // fix this using params lol
            [Command]
            public async Task SpoilerCreateAsync(params string[] name)
            {
                string channelName = name.ParseText().Replace(' ', '_');
                Methods methods = new Methods();
                if (methods.ChannelExists(Context.Guild.TextChannels, channelName))
                {
                    await ReplyAsync("This channel already exists!");
                    return;
                }

                await Context.Guild.CreateTextChannelAsync(channelName);
                await ReplyAsync("channel successfully created");
                return;
            }

            [Command("anime")]
            public async Task SpoilerCreateAnAsync(params string[] name)
            {
                // TODO actually fucking fetch from MAL and use the anime name instead of the user's input because they'll fucking break it.
                throw new NotImplementedException("NOT_IMPLEMENTED");
            }
        }


    }
}
