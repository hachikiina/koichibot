using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Discord;
using Discord.WebSocket;
using System.Runtime.InteropServices;

namespace koichibot
{
    public class Commands : ModuleBase<SocketCommandContext>
    {


        [Command("ping")]
        public async Task PingAsync()
        {
            await ReplyAsync("Pong!");
        }

        [Command("bruh")]
        public async Task BruhAsync()
        {
            await ReplyAsync("kardo anı");
        }

        [Command("avatar")]
        public async Task AvatarAsync([Optional] IGuildUser userName)
        {
            if (!(userName is null))
            {
                string avatarUrl = userName.GetAvatarUrl();
                avatarUrl = avatarUrl.Replace("?size=128", "?size=1024");

                EmbedBuilder builder = new EmbedBuilder();

                builder.WithTitle($"{ userName.Username }#{ userName.Discriminator }'s Avatar")
                    .WithImageUrl(avatarUrl)
                    .WithColor(Color.DarkPurple);

                await ReplyAsync("", false, builder.Build());
            }
            else
            {
                string avatarUrl = Context.User.GetAvatarUrl();
                avatarUrl = avatarUrl.Replace("?size=128", "?size=1024");

                EmbedBuilder builder = new EmbedBuilder();

                builder.WithTitle($"{ Context.User.Username }#{ Context.User.Discriminator }'s Avatar")
                    .WithImageUrl(avatarUrl)
                    .WithColor(Color.DarkPurple);

                await ReplyAsync("", false, builder.Build());
            }
        }
    }
}
