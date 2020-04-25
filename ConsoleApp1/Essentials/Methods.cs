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

        public async Task<string> GetAhegaoFromEg()
        {
            string json = new WebClient().DownloadString("https://raw.githubusercontent.com/egecelikci/ahegao/master/data.json");
            Ahg ahegao = JsonConvert.DeserializeObject<Ahg>(json);
            Random random = new Random();
            int rndNum = random.Next(0, ahegao.Ahegao.Length);
            return ahegao.Ahegao[rndNum];
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

    public class ExceptionList
    {
        [JsonProperty("type")]
        public Exception type { get; set; }

        [JsonProperty("data")]
        public System.Collections.IDictionary Data { get; set; }

        [JsonProperty("innerException")]
        public Exception InnerException { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("stackTrace")]
        public string StackTrace { get; set; }
    }

    public class Exceptions
    {
        [JsonProperty("exceptionList")]
        public List<ExceptionList> ExceptionList { get; set; }
    }
}
