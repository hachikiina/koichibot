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
                var color = (context.User as SocketGuildUser).GetGuildUserRoleColor();
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
                var color = (context.User as SocketGuildUser).GetGuildUserRoleColor();

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
                            .WithTimestamp(message.CreatedAt)
                            .WithColor((context.User as SocketGuildUser).GetGuildUserRoleColor());

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
    }
}
