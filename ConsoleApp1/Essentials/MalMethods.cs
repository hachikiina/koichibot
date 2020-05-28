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
    public class MalMethods
    {
        public Embed CreateAnimeEmbed(Anime anime, SocketGuildUser guildUser)
        {
            EmbedBuilder embedBuilder = new EmbedBuilder();
            embedBuilder.WithTitle(anime.Title)
                .WithDescription(anime.Synopsis)
                .WithThumbnailUrl(anime.ImageURL)
                .WithUrl("https://myanimelist.net/anime/" + anime.MalId + "/")
                .WithColor(guildUser.GetGuildUserRoleColor());

            List<EmbedFieldBuilder> fields = new List<EmbedFieldBuilder>();
            StringBuilder strBuilder = new StringBuilder();
            if (anime.Airing)
            {
                try
                {
                    strBuilder.Append(anime.Aired.From.Value.ToString("dd/MM/yyyy"))
                                .Append(" - ")
                                .Append("Still Airing");
                }
                catch (InvalidOperationException)
                {
                    strBuilder.Append("N/A");
                }

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
                    try
                    {
                        strBuilder.Append(anime.Aired.From.Value.ToString("dd/MM/yyyy"))
                                            .Append(" - ")
                                            .Append(anime.Aired.To.Value.ToString("dd/MM/yyyy"));
                    }
                    catch (InvalidOperationException)
                    {
                        strBuilder.Append("Haven't released yet.");
                    }

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
                .WithColor(guildUser.GetGuildUserRoleColor());

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
                    try
                    {
                        strBuilder.Append(manga.Published.From.Value.ToString("dd/MM/yyyy"));
                    }
                    catch (InvalidOperationException)
                    {
                        strBuilder.Append("Haven't released yet.");
                    }

                    fields.Add(new EmbedFieldBuilder { Name = "Status", Value = "Published", IsInline = true });
                    fields.Add(new EmbedFieldBuilder { Name = "Type", Value = manga.Type, IsInline = true });
                    fields.Add(new EmbedFieldBuilder { Name = "Published", Value = strBuilder.ToString(), IsInline = true });
                }
                else
                {
                    try
                    {
                        strBuilder.Append(manga.Published.From.Value.ToString("dd/MM/yyyy"))
                                            .Append(" - ")
                                            .Append(manga.Published.To.Value.ToString("dd/MM/yyyy"));
                    }
                    catch (InvalidOperationException)
                    {
                        strBuilder.Append("Haven't released yet.");
                    }

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
