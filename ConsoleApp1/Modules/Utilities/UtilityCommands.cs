using Discord;
using Discord.Commands;
using koichibot.Essentials;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using YouTubeSearch;

namespace koichibot.Modules.Utilities
{
    public class UtilityCommands : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Summary("Pong!")]
        public async Task PingAsync()
        {
            IUserMessage message = await ReplyAsync("Ping?");
            await message.ModifyAsync(m => m.Content = $"Pong!" +
            $"\nLatency is {message.Timestamp.Subtract(Context.Message.Timestamp).Milliseconds} ms." +
            $"\nAPI Latency is {Context.Client.Latency} ms.");
        }

        [Command("avatar")]
        [Summary("Gets the specified users avatar.")]
        public async Task AvatarAsync([Optional] IGuildUser username)
        {
            if (username != null)
            {
                string avatarUrl = username.GetAvatarUrl();
                avatarUrl = avatarUrl.Replace("?size=128", "?size=1024");

                EmbedBuilder builder = new EmbedBuilder();

                builder.WithTitle($"{ username.Username }#{ username.Discriminator }'s Avatar")
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

        [Command("say")]
        [Summary("Makes to bot say something!")]
        public async Task SayAsync([Optional] params string[] message)
        {
            if (message.Length == 0)
            {
                await ReplyAsync("Kullanım: `say mesaj`");
            }
            else
            {
                string final = StaticMethods.ParseText(message);

                await ReplyAsync(final);
            }
        }

        [Command("sayd")]
        [Summary("Makes the bot say something! Now deletes your command too!")]
        public async Task SayDAsync([Optional] params string[] message)
        {
            if (message.Length == 0)
            {
                await ReplyAsync("Usage: `b!sayd <message>`");
            }
            else
            {
                string final = StaticMethods.ParseText(message);

                await ReplyAsync(final);
                await Context.Message.DeleteAsync();
            }
        }

        [Command("sendmessage")]
        [Summary("Makes the bot send a message in a specified channel.")]
        public async Task SendingMessageAsync([Optional] IGuildChannel channel, [Optional] params string[] message)
        {
            if (channel is null || message.Length == 0)
            {
                await ReplyAsync("Usage: `b!sendmessage <Channel> <message>`");
                return;
            }

            string final = StaticMethods.ParseText(message);
            string lol = channel.Id.ToString();

            bool success = ulong.TryParse(lol, out ulong finalchannel);
            if (success)
            {
                await Context.Guild.GetTextChannel(finalchannel).SendMessageAsync(final);
            }
            else
            {
                await ReplyAsync("Please enter a valid channel.");
                return;
            }

        }
    }
}
