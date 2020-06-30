using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace koichibot.Essentials
{
    public static class Extensions
    {
        public static string ParseText(this string[] message)
        {
            string final = "";
            foreach (var item in message)
            {
                final = final + " " + item.ToString();
            }
            return final.Trim();
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

        public static Color GetGuildUserRoleColor(this SocketGuildUser user)
        {
            if (user.Roles.Count == 0)
            {
                return Color.Default;
            }
            else
            {
                foreach (var role in user.Roles)
                {
                    if (role.Color.RawValue != 0)
                        return role.Color;
                }
                return Color.Default;
            }
            throw new InvalidOperationException("No roles match the statements.");
        }

        public static Color GetGuildUserRoleColor(this SocketUser user)
        {
            return (user as SocketGuildUser).GetGuildUserRoleColor();
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

        /// <summary>
        /// Reads the permission for the user.
        /// </summary>
        /// <param name="guildUser"></param>
        /// <param name="permission"></param>
        /// <exception cref="DirectoryNotFoundException">Thrown when permissions directory wasn't found.</exception>
        /// <exception cref="FileNotFoundException">Thrown when permissions JSON wasn't found.</exception>
        /// <exception cref="KeyNotFoundException">Thrown when permission or the user wasn't found in the JSON file.</exception>
        /// <returns>If the user is allowed or not.</returns>
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

        public static string GetGuildUserRoles(this SocketGuildUser user)
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

        public static string GetGuildUserRoles(this SocketUser user)
        {
            return (user as SocketGuildUser).GetGuildUserRoles();
        }
    }
}
