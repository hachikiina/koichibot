using Discord;
using Discord.Commands;
using koichibot.Essentials;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace koichibot.Modules
{
    public class Utility : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        [Summary("Pong!")]
        public async Task PingAsync()
        {
            IUserMessage message = await ReplyAsync("Ping?");
            await message.ModifyAsync(m => m.Content = $"Pong!" +
            $"\nLatency is {message.Timestamp.Subtract(Context.Message.Timestamp).Milliseconds} ms." +
            $"\nAPI Latency is {Context.Client.Latency} ms.");
            return;
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
                return;
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
                return;
            }
        }

        [Command("say")]
        [Summary("Makes the bot say something.")]
        public async Task SayAsync([Optional] params string[] message)
        {

            try
            {
                if (message.Length == 0)
                {
                    await ReplyAsync("Kullanım: `say mesaj`");
                    return;
                }
                else
                {
                    string final = StaticMethods.ParseText(message);

                    await ReplyAsync(final);
                    return;
                }
            }
            catch (Exception ex)
            {
                await StaticMethods.ExceptionHandler(ex, Context.Channel);
                return;
            }
        }

        [Command("sayd")]
        [Summary("Makes the bot say something, now deletes your command too!")]
        public async Task SayDeleteAsync([Optional] params string[] message)
        {
            try
            {
                if (message.Length == 0)
                {
                    await ReplyAsync("Usage: `b!sayd <message>`");
                    return;
                }
                else
                {
                    string final = StaticMethods.ParseText(message);

                    await ReplyAsync(final);
                    await Context.Message.DeleteAsync();
                    return;
                }
            }
            catch (Exception ex)
            {
                await StaticMethods.ExceptionHandler(ex, Context.Channel);
                return;
            }
        }

        [Command("sendmessage")]
        [Summary("Makes the bot send a message in a specified text channel.")]
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
                return;
            }
            else
            {
                await ReplyAsync("Please enter a valid channel.");
                return;
            }

        }
    }
}
