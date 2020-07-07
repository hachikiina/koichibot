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
    public class Starboard : ModuleBase<SocketCommandContext>
    {
        [Group("starboard")]
        public class StarboardCommands : ModuleBase<SocketCommandContext>
        {
            // add summaries and actually fill them up
            [Command("setchannel")]
            public async Task SetStarboardChannelAsync()
            {

            }

            [Command("setthreshold")]
            public async Task SetStarboardThresholdAsync()
            {

            }
        }
    }
}
