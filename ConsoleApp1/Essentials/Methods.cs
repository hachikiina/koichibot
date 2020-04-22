using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace koichibot.Essentials
{
    public class Methods
    {
        public async Task<string> GetRandomImageFromEg()
        {
            string urlBase = "https://raw.githubusercontent.com/egecelikci/ahegao/master/images/";
            Random random = new Random();
            while (true)
            {
                int rndNum = random.Next(0, 150);
                string url = urlBase + $"{rndNum}" + ".jpg";
                if (UrlExists(url))
                {
                    return url;
                }
                else
                {
                    Console.WriteLine("doesnt exist -> " + url);
                }
            }
        }

        private bool UrlExists(string url)
        {
            WebRequest webRequest = HttpWebRequest.Create(url);
            webRequest.Method = "HEAD";
            try
            {
                using(WebResponse webResponse = webRequest.GetResponse())
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

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

        public static async Task ExceptionHandler(Exception ex, ISocketMessageChannel channel)
        {
            Console.WriteLine("\n" + ex.ToString() + "\n");
            await channel.SendMessageAsync("Something went wrong:");
            await channel.SendMessageAsync($"```{ex.ToString()}```");
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
}
