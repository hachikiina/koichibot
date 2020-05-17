using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using Serilog;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace koichibot.Essentials
{
    public class Methods
    {
        public string GetUrlFromNeko()
        {
            using WebClient webClient = new WebClient();
            string json = webClient.DownloadString("https://nekobot.xyz/api/v2/image/thighs");
            Thighs thighs = JsonConvert.DeserializeObject<Thighs>(json);
            if (thighs.Success)
            {
                return thighs.Message;
            }
            else
            {
                Console.WriteLine("Failed to get link");
                return null;
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<string> GetAhegaoFromEg()
        {
            using WebClient webClient = new WebClient();
            string json = webClient.DownloadString("https://ahegao.egecelikci.com/api");
            Ahg ahegao = JsonConvert.DeserializeObject<Ahg>(json);
            return ahegao.Msg;
        }

        public async Task<List> GetUrbanQuery(string query)
        {
            using WebClient webClient = new WebClient();
            string json = webClient.DownloadString("http://api.urbandictionary.com/v0/define?term=" + query.Replace(" ", "%20"));
            UrbanDefine response = JsonConvert.DeserializeObject<UrbanDefine>(json);
            //return response.List.First();
            if (response.List.Count == 0)
                throw new ArgumentNullException("query", "Couldn't find the definition");

            return response.List.First();
        }

        /// <summary>
        /// Gets the first colored role user has. If there are none, returns Color.Default
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<Discord.Color> GetGuildUserRoleColor(SocketGuildUser user)
        {
            if (user.Roles.Count == 0)
            {
                return Discord.Color.Default;
            }
            else
            {
                foreach (var role in user.Roles)
                {
                    //Console.WriteLine(role.Color.ToString() + "\n" + role.Color.RawValue + "\n");
                    if (role.Color.RawValue != 0)
                        return role.Color;
                }
                return Discord.Color.Default;
            }
            throw new InvalidOperationException("No roles match the statements.");
        }

        /// <summary>
        /// Gets a guild user's roles and puts lists them as a string.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> GetGuildUserRoles(SocketGuildUser user)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            StringBuilder rolesBuilder = new StringBuilder();
            StringBuilder tempBuilder = new StringBuilder();
            int i = 0;

            foreach (var role in user.Roles)
            {
                if (i == user.Roles.Count - 1)
                    tempBuilder.Append(role.Name);
                else
                    tempBuilder.Append(role.Name + ", ");

                rolesBuilder = tempBuilder;
                i++;
            }

            return rolesBuilder.ToString();
        }

        [Obsolete("This method is deprecated, please use DrawRegularPolygonSK instead.", true)]
        public string DrawRegularPolygon(int vertexCount, float radius, SocketCommandContext context)
        {
            string imagesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "images");
            if (!Directory.Exists(imagesPath))
                Directory.CreateDirectory(imagesPath);

            using (Bitmap bmp = new Bitmap(1024, 1024))
            using (Graphics graphics = Graphics.FromImage(bmp))
            {

                graphics.Clear(System.Drawing.Color.Transparent);
                PointF center = new PointF(512f, 512f);

                var angle = Math.PI * 2 / vertexCount;
                var points = Enumerable.Range(0, vertexCount).Select(i => PointF.Add(center, new SizeF((float)Math.Sin(i * angle) * radius,
                    (float)Math.Cos(i * angle) * radius)));
                var color = GetGuildUserRoleColor(context.User as SocketGuildUser).Result;
                var brush = new SolidBrush(System.Drawing.Color.FromArgb(255, color.R, color.G, color.B));

                graphics.FillPolygon(brush, points.ToArray());
                graphics.DrawPolygon(new Pen(Brushes.Black, 5f), points.ToArray());

                bmp.Save(Path.Combine(imagesPath, context.Message.Id + ".png"), System.Drawing.Imaging.ImageFormat.Png);
                return Path.Combine(imagesPath, context.Message.Id + ".png");
            }
        }

        public string DrawRegularPolygonSK(int vertexCount, float radius, SocketCommandContext context, [Optional] int rndNum)
        {
            string imagesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "images");
            if (!Directory.Exists(imagesPath))
                Directory.CreateDirectory(imagesPath);

            using (SKBitmap bmp       = new SKBitmap(1024, 1024))
            using (SKPaint outerPaint = new SKPaint())
            using (SKPaint innerPaint = new SKPaint())
            using (SKPaint textPaint  = new SKPaint())
            using (SKCanvas canvas    = new SKCanvas(bmp))
            {
                SKPoint center = new SKPoint(bmp.Width / 2, bmp.Height / 2);
                var angle = Math.PI * 2 / vertexCount;
                var points = Enumerable.Range(0, vertexCount).Select(i => SKPoint.Add(center, new SKSize((float)Math.Sin(i * angle) * radius,
                    (float)Math.Cos(i * angle) * radius)));
                var color = GetGuildUserRoleColor(context.User as SocketGuildUser).Result;
                
                canvas.Clear();

                // this is for the filling of the polygon.
                innerPaint.Color = new SKColor(color.R, color.G, color.B);
                innerPaint.Style = SKPaintStyle.Fill;
                innerPaint.StrokeWidth = 5f;
                SKPath innerPath = new SKPath();
                foreach (var point in points)
                {
                    if (point == points.First())
                        innerPath.MoveTo(point);
                    else
                        innerPath.LineTo(point);
                }
                innerPath.Close();
                canvas.DrawPath(innerPath, innerPaint);

                // this is for the lines of the polygon.
                outerPaint.Color = SKColors.Black;
                outerPaint.StrokeWidth = 10f;
                outerPaint.StrokeCap = SKStrokeCap.Round;
                outerPaint.Style = SKPaintStyle.Stroke;
                SKPath outerPath = new SKPath();
                foreach (var point in points)
                {
                    if (point == points.First())
                        outerPath.MoveTo(point);
                    else
                        outerPath.LineTo(point);
                }
                outerPath.Close();
                canvas.DrawPath(outerPath, outerPaint);

                // this is for the text.
                textPaint.TextSize = 240f;
                textPaint.IsAntialias = true;

                if (color.R * 0.299f + color.G * 0.587f + color.B * 0.114f > 150) // decides the color of text based on bg color
                    textPaint.Color = SKColors.Black;
                else
                    textPaint.Color = SKColors.White;

                textPaint.Style = SKPaintStyle.StrokeAndFill;
                textPaint.StrokeWidth = 15f;
                textPaint.TextAlign = SKTextAlign.Center;
                textPaint.GetFontMetrics(out var skFontMetrics);
                canvas.DrawText(rndNum.ToString(), center.X, center.Y + skFontMetrics.CapHeight / 2, textPaint); // text's height is 188 pixels at size 240f and stroke 15f

                using (var stream = File.Create(Path.Combine(imagesPath, context.Message.Id.ToString() + ".png")))
                {
                    SKData data = SKImage.FromBitmap(bmp).Encode(SKEncodedImageFormat.Png, 100);
                    data.SaveTo(stream);
                }

                return Path.Combine(imagesPath, context.Message.Id.ToString() + ".png").ToString();
            }
        }

        public async Task QuoteAsync(SocketCommandContext context, string messageContent)
        {
            try
            {
                string messageUrl = messageContent.Split(' ').First().ToLower().Substring(messageContent.Split(' ').First().Length - 56);
                string[] ids = messageUrl.Split('/');
                if (!ulong.TryParse(ids[0], out ulong guildID) || !ulong.TryParse(ids[1], out ulong channelID))
                {
                    await context.Channel.SendMessageAsync("Please provide a valid url.\nCheck the link again.");
                    return;
                }

                IMessage message;
                if (context.Guild.Id == guildID && context.Channel.Id == channelID)
                {
                    try
                    {
                        message = await context.Channel.GetMessageAsync(ulong.Parse(ids[2]));
                        Methods methods = new Methods();

                        EmbedBuilder embedBuilder = new EmbedBuilder();
                        embedBuilder.WithAuthor(message.Author)
                            .WithDescription(message.Content)
                            .AddField("Quoted by", $"{context.Message.Author.Mention} from [{message.Channel.Name}]({message.GetJumpUrl()})")
                            .WithCurrentTimestamp()
                            .WithColor(await methods.GetGuildUserRoleColor(context.User as SocketGuildUser));

                        if (message.Attachments.Count > 0)
                            embedBuilder.WithImageUrl(message.Attachments.First().ProxyUrl);

                        await context.Channel.SendMessageAsync("", false, embedBuilder.Build());

                        if (!(messageContent.Split(' ').Count() > 1))
                            await context.Message.DeleteAsync();

                        return;
                    }
                    catch (NullReferenceException)
                    {
                        await context.Channel.SendMessageAsync("Couldn't find the message in guild. Are you sure you're entering the link correctly?");
                        return;
                    }
                }
                else
                {
                    await context.Channel.SendMessageAsync("This url is either not referencing a message in this server or is invalid.");
                    return;
                }
            }
            catch (Exception ex)
            {
                await StaticMethods.ExceptionHandler(ex, context);
                return;
            }
        }
    }

    public static class StaticMethods
    {
        public static ulong OwnerID = 309758882425733121;
        public static string ParseText(this string[] message)
        {
            string final = "";
            foreach (var item in message)
            {
                final = final + " " + item.ToString();
            }
            return final.Trim();
        }

        public static async Task ExceptionHandler(Exception ex, SocketCommandContext context)
        {
            await context.Channel.SendMessageAsync($"Something went wrong, `{ex.GetType()}`");
            //await context.Channel.SendMessageAsync($"```{ex.ToString()}```");
            Log.Error($"Something went wrong at {context.Guild.Name}/{context.Channel.Name}");
            Log.Error($"Message content: \"{context.Message.Content}\"");
            Log.Error($"({context.Guild.Id}/{context.Channel.Id}/{context.Message.Id})");
            Log.Error(ex, "Details: ");
            Log.Error("----------------------------------------------");
            return;
        }

        public static bool IsGuildUser(this IGuild guild, string user)
        {
            foreach (var guildUser in guild.GetUsersAsync().Result)
            {
                if (guildUser.Username == user || guildUser.Nickname == user || guildUser.Id.ToString() == user)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class Thighs
    {
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("success")]
        public bool Success { get; set; }
    }

    public class Ahg
    {
        [JsonProperty("msg")]
        public string Msg { get; set; }
    }

    public class UrbanDefine
    {
        [JsonProperty("list")]
        public List<List> List { get; set; }
    }

    public partial class List
    {
        [JsonProperty("defid")]
        public long Defid { get; set; }

        [JsonProperty("word")]
        public string Word { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("permalink")]
        public Uri Permalink { get; set; }

        [JsonProperty("definition")]
        public string Definition { get; set; }

        [JsonProperty("example")]
        public string Example { get; set; }

        [JsonProperty("thumbs_up")]
        public long ThumbsUp { get; set; }

        [JsonProperty("thumbs_down")]
        public long ThumbsDown { get; set; }

        [JsonProperty("current_vote")]
        public string CurrentVote { get; set; }
    }

    public class Permissions
    {
        [JsonProperty("guild")]
        public Dictionary<IUserGuild, List<string>> Guild { get; set; }

        [JsonProperty("guildChannel")]
        public Dictionary<IGuildChannel, List<string>> GuildChannel { get; set; }

        [JsonProperty("guildChannelUser")]
        public Dictionary<IGuildChannel, Dictionary<IGuildUser, List<string>>> GuildChannelUser { get; set; }

    }
}
