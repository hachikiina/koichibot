using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.IO;
using Serilog;
using System.Text;
using System.Drawing;

namespace koichibot.Essentials
{
    public class Methods
    {
        public string GetUrlFromNeko()
        {
            string json = new WebClient().DownloadString("https://nekobot.xyz/api/v2/image/thighs");
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
            string json = new WebClient().DownloadString("https://ahegao.egecelikci.com/api");
            Ahg ahegao = JsonConvert.DeserializeObject<Ahg>(json);
            return ahegao.Msg;
        }

        public async Task<List> GetUrbanQuery(string query)
        {
            string json = new WebClient().DownloadString("http://api.urbandictionary.com/v0/define?term=" + query.Replace(" ", "%20"));
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

        public string DrawRegularPolygon(int vertexCount, float radius, SocketCommandContext context)
        {
            using (Bitmap bmp = new Bitmap(1024, 1024))
            using (Graphics graphics = Graphics.FromImage(bmp))
            {
                string imagesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "images");
                if (!Directory.Exists(imagesPath))
                    Directory.CreateDirectory(imagesPath);

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
        [JsonProperty("ahegao")]
        public string[] Ahegao { get; set; }
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
}
