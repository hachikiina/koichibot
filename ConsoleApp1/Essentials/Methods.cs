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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using JikanDotNet;

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
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

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

            using (SKBitmap bmp = new SKBitmap(1024, 1024))
            using (SKPaint outerPaint = new SKPaint())
            using (SKPaint innerPaint = new SKPaint())
            using (SKPaint textPaint = new SKPaint())
            using (SKCanvas canvas = new SKCanvas(bmp))
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

        public async Task QuoteAsync(SocketCommandContext context, string jumpUrl, List<string> messageContent)
        {
            try
            {
                string messageUrl = jumpUrl.Split(' ').First().ToLower().Substring(jumpUrl.Split(' ').First().Length - 56);
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

                        if (!(messageContent.Count() > 1))
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

        public Embed CreateAnimeEmbed(Anime anime, SocketGuildUser guildUser)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(anime.Title)
                .WithDescription(anime.Synopsis)
                .WithThumbnailUrl(anime.ImageURL)
                .WithUrl("https://myanimelist.net/anime/" + anime.MalId + "/")
                .WithColor(GetGuildUserRoleColor(guildUser).Result);

            List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
            StringBuilder strBuilder = new StringBuilder();
            if (anime.Airing)
            {
                strBuilder.Append(anime.Aired.From.Value.ToString("dd/MM/yyyy"))
                    .Append(" - ")
                    .Append("Still Airing");

                fields.Add(new EmbedFieldBuilder { Name = "Status", Value = "Airing", IsInline = true });
                fields.Add(new EmbedFieldBuilder { Name = "Type", Value = anime.Type, IsInline = true });
                fields.Add(new EmbedFieldBuilder { Name = "Aired", Value = strBuilder.ToString(), IsInline = false });
            }
            else
            {
                if (anime.Type.ToLower() == "movie")
                {
                    strBuilder.Append(anime.Aired.From.Value.ToString("dd/MM/yyyy"));

                    fields.Add(new EmbedFieldBuilder { Name = "Status", Value = "Finished", IsInline = true });
                    fields.Add(new EmbedFieldBuilder { Name = "Type", Value = anime.Type, IsInline = true });
                    fields.Add(new EmbedFieldBuilder { Name = "Released", Value = strBuilder.ToString(), IsInline = false });
                }
                else
                {
                    strBuilder.Append(anime.Aired.From.Value.ToString("dd/MM/yyyy"))
                            .Append(" - ")
                            .Append(anime.Aired.To.Value.ToString("dd/MM/yyyy"));

                    fields.Add(new EmbedFieldBuilder { Name = "Status", Value = "Finished", IsInline = true });
                    fields.Add(new EmbedFieldBuilder { Name = "Type", Value = anime.Type, IsInline = true });
                    fields.Add(new EmbedFieldBuilder { Name = "Aired", Value = strBuilder.ToString(), IsInline = false });
                }
            }

            if (anime.Rank != null)
                fields.Add(new EmbedFieldBuilder { Name = "Rank", Value = anime.Rank, IsInline = true });
            else
                fields.Add(new EmbedFieldBuilder { Name = "Rank", Value = "N/A", IsInline = true });

            if (anime.Score != null)
                fields.Add(new EmbedFieldBuilder { Name = "Score", Value = anime.Score, IsInline = true });
            else
                fields.Add(new EmbedFieldBuilder { Name = "Score", Value = "N/A", IsInline = true });

            if (anime.ScoredBy != null)
                fields.Add(new EmbedFieldBuilder { Name = "Scored by", Value = anime.ScoredBy + " people", IsInline = true });
            else
                fields.Add(new EmbedFieldBuilder { Name = "Scored by", Value = "N/A", IsInline = true });

            embedBuilder.WithFields(fields);
            return embedBuilder.Build();
        }

        public Embed CreateAnimeEmbed(Anime anime, SocketUser user)
        {
            return CreateAnimeEmbed(anime, user as SocketGuildUser);
        }

        public Embed CreateAnimeEmbed(Anime anime, IGuildUser guildUser)
        {
            return CreateAnimeEmbed(anime, guildUser as SocketGuildUser);
        }

        public Embed CreateMangaEmbed(Manga manga, SocketGuildUser guildUser)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(manga.Title)
                .WithDescription(manga.Synopsis)
                .WithThumbnailUrl(manga.ImageURL)
                .WithUrl("https://myanimelist.net/manga/" + manga.MalId + "/")
                .WithColor(GetGuildUserRoleColor(guildUser).Result);

            List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
            StringBuilder strBuilder = new StringBuilder();
            if (manga.Publishing)
            {
                strBuilder.Append(manga.Published.From.Value.ToString("dd/MM/yyyy"))
                    .Append(" - ")
                    .Append("Still Publishing.");

                fields.Add(new EmbedFieldBuilder { Name = "Status", Value = "Publishing", IsInline = true });
                fields.Add(new EmbedFieldBuilder { Name = "Type", Value = manga.Type, IsInline = true });
                fields.Add(new EmbedFieldBuilder { Name = "Published", Value = strBuilder.ToString(), IsInline = true });
            }
            else
            {
                if (manga.Type.ToLower() == "novel")
                {
                    strBuilder.Append(manga.Published.From.Value.ToString("dd/MM/yyyy"));

                    fields.Add(new EmbedFieldBuilder { Name = "Status", Value = "Published", IsInline = true });
                    fields.Add(new EmbedFieldBuilder { Name = "Type", Value = manga.Type, IsInline = true });
                    fields.Add(new EmbedFieldBuilder { Name = "Published", Value = strBuilder.ToString(), IsInline = true });
                }
                else
                {
                    strBuilder.Append(manga.Published.From.Value.ToString("dd/MM/yyyy"))
                            .Append(" - ")
                            .Append(manga.Published.To.Value.ToString("dd/MM/yyyy"));

                    fields.Add(new EmbedFieldBuilder { Name = "Status", Value = "Finished", IsInline = true });
                    fields.Add(new EmbedFieldBuilder { Name = "Type", Value = manga.Type, IsInline = true });
                    fields.Add(new EmbedFieldBuilder { Name = "Published", Value = strBuilder.ToString(), IsInline = true }); 
                }
            }

            if (manga.Rank != null)
                fields.Add(new EmbedFieldBuilder { Name = "Rank", Value = manga.Rank, IsInline = true });
            else
                fields.Add(new EmbedFieldBuilder { Name = "Rank", Value = "N/A", IsInline = true });

            if (manga.Score != null)
                fields.Add(new EmbedFieldBuilder { Name = "Score", Value = manga.Score, IsInline = true });
            else
                fields.Add(new EmbedFieldBuilder { Name = "Score", Value = "N/A", IsInline = true });

            if (manga.ScoredBy != null)
                fields.Add(new EmbedFieldBuilder { Name = "Scored by", Value = manga.ScoredBy, IsInline = true });
            else
                fields.Add(new EmbedFieldBuilder { Name = "Scored by", Value = "N/A", IsInline = true });

            embedBuilder.WithFields(fields);
            return embedBuilder.Build();
        }

        public Embed CreateMangaEmbed(Manga manga, SocketUser user)
        {
            return CreateMangaEmbed(manga, user as SocketGuildUser);
        }

        public Embed CreateMangaEmbed(Manga manga, IGuildUser guildUser)
        {
            return CreateMangaEmbed(manga, guildUser as SocketGuildUser);
        }
    }
}
