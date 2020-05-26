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

        public static void WritePermission(this IGuildUser guildUser, string permission, bool allow)
        {
            string jsonDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "json", "permissions");
            if (!Directory.Exists(jsonDir))
                Directory.CreateDirectory(jsonDir);

            var perms = new Permissions();
            string jsonPath = Path.Combine(jsonDir, guildUser.GuildId + ".json");
            if (!File.Exists(jsonPath))
            {
                var temp = new Dictionary<string, bool>();
                temp.Add(permission, allow);
                perms.UserPermissionPairs = new Dictionary<ulong, Dictionary<string, bool>>();
                perms.UserPermissionPairs.Add(guildUser.Id, temp);

                using (var stream = new StreamWriter(File.Create(jsonPath)))
                    stream.WriteLine(JsonConvert.SerializeObject(perms, Formatting.Indented));
            }
            else
            {
                using (var streamReader = new StreamReader(jsonPath))
                {
                    perms = JsonConvert.DeserializeObject<Permissions>(streamReader.ReadToEnd());
                    if (perms.UserPermissionPairs.ContainsKey(guildUser.Id))
                    {
                        if (perms.UserPermissionPairs.GetValueOrDefault(guildUser.Id).ContainsKey(permission))
                            perms.UserPermissionPairs.GetValueOrDefault(guildUser.Id)[permission] = allow;
                        else
                            perms.UserPermissionPairs.GetValueOrDefault(guildUser.Id).Add(permission, allow);
                    }
                    else
                    {
                        perms.UserPermissionPairs.Add(guildUser.Id, new Dictionary<string, bool>() { { permission, allow } });
                    }

                }
                using (var streamWriter = new StreamWriter(jsonPath))
                {
                    streamWriter.WriteLine(JsonConvert.SerializeObject(perms, Formatting.Indented));
                }
            }
        }

        public static bool ReadPermission(this IGuildUser guildUser, string permission)
        {
            string jsonDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "json", "permissions");
            if (!Directory.Exists(jsonDir))
                throw new DirectoryNotFoundException("Permissions directory was not found.");

            string jsonPath = Path.Combine(jsonDir, guildUser.GuildId + ".json");
            if (!File.Exists(jsonPath))
                throw new FileNotFoundException("Permissions JSON for the guild was not found.");

            var perms = new Permissions();
            using (var streamReader = new StreamReader(jsonPath))
                perms = JsonConvert.DeserializeObject<Permissions>(streamReader.ReadToEnd());

            if (perms.UserPermissionPairs.ContainsKey(guildUser.Id))
            {
                if (perms.UserPermissionPairs.GetValueOrDefault(guildUser.Id).ContainsKey(permission))
                    return perms.UserPermissionPairs.GetValueOrDefault(guildUser.Id).GetValueOrDefault(permission);
                else
                    throw new KeyNotFoundException("Permission was not found in the JSON.");
            }
            else
            {
                throw new KeyNotFoundException("User was not found in the JSON.");
            }
        }
    }

}
