using Discord;
using Discord.Commands;
using Discord.WebSocket;
using koichibot.Essentials;
using System;
using System.Runtime.InteropServices;
using System.Text;
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
            // still doesn't get to method body if the given arg isn't a user.
            try
            {
                if (username != null)
                {
                    foreach (var user in Context.Guild.Users)
                    {
                        if (username == user)
                        {
                            string avatarUrl = username.GetAvatarUrl().Replace("?size=128", "?size=1024");

                            EmbedBuilder builder = new EmbedBuilder();

                            builder.WithTitle($"{ username.Username }#{ username.Discriminator }'s Avatar")
                                .WithDescription($"[Direct Link]({avatarUrl})")
                                .WithImageUrl(avatarUrl)
                                .WithColor(Context.User.GetGuildUserRoleColor());

                            await ReplyAsync("", false, builder.Build());
                            return;
                        }
                        else continue;
                    }
                    await ReplyAsync("Couldn't find the user.");
                    return;
                }
                else
                {
                    string avatarUrl = Context.User.GetAvatarUrl().Replace("?size=128", "?size=1024");

                    EmbedBuilder builder = new EmbedBuilder();

                    builder.WithTitle($"{ Context.User.Username }#{ Context.User.Discriminator }'s Avatar")
                        .WithDescription($"[Direct Link]({avatarUrl})")
                        .WithImageUrl(avatarUrl)
                        .WithColor(Context.User.GetGuildUserRoleColor());

                    await ReplyAsync("", false, builder.Build());
                    return;
                }
            }
            catch (Exception ex)
            {
                await StaticMethods.ExceptionHandler(ex, Context);
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
                    string final = message.ParseText();

                    await ReplyAsync(final);
                    return;
                }
            }
            catch (Exception ex)
            {
                await StaticMethods.ExceptionHandler(ex, Context);
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
                    string final = message.ParseText();

                    await ReplyAsync(final);
                    await Context.Message.DeleteAsync();
                    return;
                }
            }
            catch (Exception ex)
            {
                await StaticMethods.ExceptionHandler(ex, Context);
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

            string final = message.ParseText();
            string channelId = channel.Id.ToString();

            bool success = ulong.TryParse(channelId, out ulong finalchannel);
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

        #region oldQuote deprecated
        //// this command is very much WIP => should also be able to accept message id's
        //[Command("quote")]
        //[Summary("Quotes the message within the given link, must be a Discord message link.")]
        //public async Task QuoteAsync([Optional] params string[] messageUrl)
        //{
        //    if (messageUrl.Length == 0)
        //    {
        //        await ReplyAsync("Usage: `b!quote <messageUrl>`");
        //        return;
        //    }
        //    try
        //    {
        //        string opString = messageUrl.ParseText();
        //        if (!opString.ToLower().Contains("https://discordapp.com/channels/"))
        //        {
        //            await ReplyAsync("Please provide a valid url.");
        //            return;
        //        }

        //        opString = opString.ToLower().Substring(opString.Length - 56);
        //        string[] ids = opString.Split('/');
        //        if (!ulong.TryParse(ids[0], out ulong guildID) || !ulong.TryParse(ids[1], out ulong channelID))
        //        {
        //            await ReplyAsync("Please provide a valid url.\nCheck the link again.");
        //            return;
        //        }

        //        IMessage message;
        //        if (Context.Guild.Id == guildID && Context.Channel.Id == channelID)
        //        {
        //            try
        //            {
        //                message = await Context.Channel.GetMessageAsync(ulong.Parse(ids[2]));
        //                Methods methods = new Methods();

        //                EmbedBuilder embedBuilder = new EmbedBuilder();
        //                embedBuilder.WithAuthor(message.Author)
        //                    .WithDescription(message.Content)
        //                    .AddField("Quoted by", $"{Context.Message.Author.Mention} from [{message.Channel.Name}]({message.GetJumpUrl()})")
        //                    .WithCurrentTimestamp()
        //                    .WithColor(await methods.GetGuildUserRoleColor(Context.User as SocketGuildUser));

        //                if (message.Attachments.Count > 0)
        //                    embedBuilder.WithImageUrl(message.Attachments.First().ProxyUrl);

        //                await ReplyAsync("", false, embedBuilder.Build());

        //                return;
        //            }
        //            catch (NullReferenceException)
        //            {
        //                await ReplyAsync("Couldn't find the message in guild. Are you sure you're entering the link correctly?");
        //                return;
        //            }
        //        }
        //        else
        //        {
        //            await ReplyAsync("This url is either not referencing a message in this server or is invalid.");
        //            return;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await StaticMethods.ExceptionHandler(ex, Context);
        //        return;
        //    }
        //} 
        #endregion

        [Command("server")]
        [Alias("serverinfo")]
        [Summary("Displays server related information.")]
        public async Task ServerInfoAsync([Optional] params string[] query)
        {
            try
            {
                Methods methods = new Methods();
                EmbedBuilder embedBuilder = new EmbedBuilder();
                // role builder
                if (query.ParseText() == "roles")
                {
                    StringBuilder rolesBuilder = new StringBuilder();
                    StringBuilder tempBuilder = new StringBuilder();
                    int i = 0;
                    foreach (var role in Context.Guild.Roles)
                    {
                        if (i == Context.Guild.Roles.Count - 1)
                            tempBuilder.Append(role.Name);
                        else
                            tempBuilder.Append(role.Name + ", ");

                        // will probably throw a exception if the length is over 1024, too lazy to fix though
                        if (tempBuilder.Length >= 1024)
                        {
                            rolesBuilder = rolesBuilder.Remove(1023, rolesBuilder.Length);
                            break;
                        }

                        rolesBuilder = tempBuilder;
                        i++;
                    }
                    embedBuilder.WithTitle($"Roles [{Context.Guild.Roles.Count}]")
                        .WithDescription(rolesBuilder.ToString())
                        .WithColor(Context.User.GetGuildUserRoleColor());

                    await ReplyAsync("", false, embedBuilder.Build());
                }
                else
                {
                    embedBuilder.WithAuthor(Context.Guild.Name, Context.Guild.IconUrl)
                            .WithThumbnailUrl(Context.Guild.IconUrl)
                            .AddField("ID", Context.Guild.Id, true)
                            .AddField("Region", Context.Guild.VoiceRegionId, true)
                            .AddField("Member count", Context.Guild.MemberCount, true)
                            .AddField("Channels", $"{Context.Guild.TextChannels.Count} Text / {Context.Guild.VoiceChannels.Count} Voice", true)
                            .AddField("Owner", Context.Guild.Owner.ToString() + $"\n[{Context.Guild.Owner.Id}]", true);


                    embedBuilder.AddField("Roles", "To see the prompt with list of roles, please run `b!server roles`", false);

                    embedBuilder.WithFooter("Created at: " + Context.Guild.CreatedAt.UtcDateTime.ToString())
                        .WithColor(Context.User.GetGuildUserRoleColor());

                    await ReplyAsync("", false, embedBuilder.Build());
                    return;
                }
            }
            catch (Exception ex)
            {
                await StaticMethods.ExceptionHandler(ex, Context);
                return;
            }
        }


        // find a way to actually display an error if no match
        [Command("userinfo")]
        [Summary("Displays user related information.")]
        public async Task UserInfoAsync([Optional] IGuildUser guildUser)
        {
            var user = guildUser as SocketGuildUser ?? Context.User as SocketGuildUser;
            try
            {
                EmbedBuilder embedBuilder = new EmbedBuilder();
                Methods methods = new Methods();

                embedBuilder.WithAuthor(user)
                    .WithThumbnailUrl(user.GetAvatarUrl())
                    .AddField("ID", user.Id, true)
                    .AddField("Mention", user.Mention, true)
                    .AddField("Joined at", user.JoinedAt.Value.UtcDateTime, false)
                    .AddField("Created at", user.CreatedAt.UtcDateTime, false)
                    .AddField($"Roles [{user.Roles.Count}]", user.GetGuildUserRoles(), false)
                    .WithColor(user.GetGuildUserRoleColor());

                await ReplyAsync("", false, embedBuilder.Build());
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
