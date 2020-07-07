using System.IO;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using Discord.Rest;
using Discord.Net;
using koichibot.Essentials;
using Newtonsoft.Json;

namespace koichibot.Base
{
    public class EventHandlers
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _serviceProvider;
        private readonly string StarboardDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "json", "starboardmessages");
        private readonly string settingsDir = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "json", "starboardmessages"), "starboardsettings.json");
        private readonly string messagesDir = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "json", "starboardmessages"), "starboardmessages.json");

        public EventHandlers(DiscordSocketClient client, CommandService commands, IServiceProvider serviceProvider)
        {
            _client = client;
            _commands = commands;
            _serviceProvider = serviceProvider;
        }

        public async Task InstallEventsAsync()
        {
            StarboardFileCheck(); // checks if the files exist before doing anything

            _client.ReactionAdded += ReactionAdded;
            _client.ReactionRemoved += ReactionRemoved;
            _client.ReactionsCleared += ReactionsCleared;

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _serviceProvider); // original at program.cs
        }

        private Task ReactionsCleared(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            throw new NotImplementedException();
        }

        private Task ReactionRemoved(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            throw new NotImplementedException();
        }

        private Task ReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            throw new NotImplementedException();
        }

        private void StarboardFileCheck()
        {
            if (!Directory.Exists(StarboardDirectory))
                Directory.CreateDirectory(StarboardDirectory);

            if (!File.Exists(settingsDir))
                File.Create(settingsDir);

            if (!File.Exists(messagesDir))
                File.Create(messagesDir);
        }

        private StarboardMessages GetStarboardMessages()
        {
            if (StaticMethods.IsFileEmpty(messagesDir))
            {
                using (StreamWriter sw = new StreamWriter(messagesDir))
                    sw.WriteLine(JsonConvert.SerializeObject(new StarboardMessages { Messages = new List<StarboardMessage>() }, Formatting.Indented));
            }

            throw new NotImplementedException();
        }

        private void UpdateStarboardMessages(SocketMessage starredMessage)
        {
            if (!starredMessage.Reactions.ContainsKey(new Emoji("⭐")))
                return;

            starredMessage.Reactions.TryGetValue(new Emoji("⭐"), out ReactionMetadata reactionMetadata);
            int starCount = reactionMetadata.ReactionCount;

            if (!StaticMethods.IsFileEmpty(messagesDir))
            {
                using (FileStream fs = new FileStream(messagesDir, FileMode.Open, FileAccess.ReadWrite))
                using (StreamReader sr = new StreamReader(fs))
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    var messages = JsonConvert.DeserializeObject<StarboardMessages>(sr.ReadToEnd().Replace("\n", ""));

                    foreach (var starboardMessage in messages.Messages)
                    {
                        if (starboardMessage.Message == starredMessage)
                        {
                            starboardMessage.StarCount = starCount;
                            sw.WriteLine(JsonConvert.SerializeObject(messages, Formatting.Indented));
                            return;
                        }
                    }

                    messages.Messages.Append(new StarboardMessage() { Message = starredMessage, StarCount = starCount });
                    sw.WriteLine(JsonConvert.SerializeObject(messages, Formatting.Indented));
                    return;
                }
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(messagesDir))
                {
                    sw.WriteLine(JsonConvert.SerializeObject(new StarboardMessages { Messages = new List<StarboardMessage>() { new StarboardMessage() { Message = starredMessage, StarCount = starCount } } }));
                    return;
                }
            }
        }
    }
}
