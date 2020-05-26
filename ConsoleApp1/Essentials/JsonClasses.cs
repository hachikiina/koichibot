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

namespace koichibot.Essentials
{

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

    public class SettingsJson
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("prefix")]
        public string Prefix { get; set; }
    }

    public class Permissions
    {
        [JsonProperty("userPermissionPairs")]
        public Dictionary<ulong, Dictionary<string, bool>> UserPermissionPairs { get; set; }
    }
}
