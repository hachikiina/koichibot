using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace koichibot
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
    }

    public static class StaticMethods
    {
        public static ulong OwnerID = 309758882425733121;
        public static string ParseText(params string[] message)
        {
            string final = "";
            foreach (var item in message)
            {
                final = final + " " + item.ToString();
            }
            return final;
        }
    }
}
