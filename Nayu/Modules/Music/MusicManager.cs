﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Nayu.Core.Handlers;
using Nayu.Helpers;
using Victoria;
using Victoria.Enums;
using Victoria.EventArgs;

namespace Nayu.Modules.Music {
    public class MusicManager
    {
        private readonly LavaNode _lavaNode;
        private readonly Logger _logger;
        public readonly HashSet<ulong> VoteQueue;
        private const string Format1 = @"hh\.mm\:ss";
        private const string Format2 = @"mm\.ss";
        public MusicManager(LavaNode lavaNode, Logger logger)
        {
            _logger = logger;
            _lavaNode = lavaNode;
            _lavaNode.OnLog += OnLog;
            _lavaNode.OnPlayerUpdated += OnPlayerUpdated;
            _lavaNode.OnStatsReceived += OnStatsReceived;
            _lavaNode.OnTrackEnded += OnTrackEnded;
            _lavaNode.OnTrackException += OnTrackException;
            _lavaNode.OnTrackStuck += OnTrackStuck;
            _lavaNode.OnWebSocketClosed += OnWebSocketClosed;

            VoteQueue = new HashSet<ulong>();
        }

        private Task OnLog(LogMessage arg) {
            _logger.ConsoleMusicLog(arg.Exception + arg.Message);
            return Task.CompletedTask;
        }

        private Task OnPlayerUpdated(PlayerUpdateEventArgs arg) {
            _logger.ConsoleMusicLog($"Player update received for {arg.Player.VoiceChannel.Name}.");
            return Task.CompletedTask;
        }

        private Task OnStatsReceived(StatsEventArgs arg) {
            _logger.ConsoleMusicLog($"Lavalink Uptime {arg.Uptime}.");
            return Task.CompletedTask;
        }

        private async Task OnTrackEnded(TrackEndedEventArgs args) {
            if (!args.Reason.ShouldPlayNext())
                return;

            var player = args.Player;
            if (!player.Queue.TryDequeue(out var queueable)) {
                await player.TextChannel.SendMessageAsync("No more tracks to play.");
                return;
            }

            if (!(queueable is LavaTrack track)) {
                await player.TextChannel.SendMessageAsync("Next item in queue is not a track.");
                return;
            }

            VoteQueue.Clear();
            
            await args.Player.PlayAsync(track);
            await args.Player.TextChannel.SendMessageAsync(
                $"{args.Reason}: {args.Track.Title}\nNow playing: {track.Title}");
        }

        private Task OnTrackException(TrackExceptionEventArgs arg) {
            _logger.ConsoleMusicLog($"Track exception received for {arg.Track.Title}.");
            return Task.CompletedTask;
        }

        private Task OnTrackStuck(TrackStuckEventArgs arg) {
            _logger.ConsoleMusicLog($"Track stuck received for {arg.Track.Title}.");
            return Task.CompletedTask;
        }

        private Task OnWebSocketClosed(WebSocketClosedEventArgs arg) {
            _logger.ConsoleMusicLog($"Discord WebSocket connection closed with following reason: {arg.Reason}");
            return Task.CompletedTask;
        }
        
        
        public async Task<Embed> GetQueueAsync(ShardedCommandContext context)
        {
            if (!_lavaNode.TryGetPlayer(context.Guild, out var player))
            {
                return EmbedHandler.CreateEmbed(context, "Music Queue",
                    $"{Global.ENo} | I'm not connected to a voice channel.", EmbedHandler.EmbedMessageType.Error);
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                return EmbedHandler.CreateEmbed(context, "Music Queue",
                    $"{Global.ENo} | There's nothing playing at the moment", EmbedHandler.EmbedMessageType.Error);
            }

            var currentTrack = player.Track;
            var artwork = await currentTrack.FetchArtworkAsync();

            var descriptionBuilder = new StringBuilder();

            if (player.Queue.Count < 1 && player.PlayerState != PlayerState.Playing)
            {
                string title = currentTrack.Title;
                return EmbedHandler.CreateEmbed(context, $"Now Playing: **{title}**",
                    "There are no other items in the queue.", EmbedHandler.EmbedMessageType.Success);
            }

            var trackNum = 2;
            foreach (LavaTrack track in player.Queue.Items)
            {
                if (trackNum == 2)
                {
                    descriptionBuilder.Append($"Up Next: **[{track.Title}]** - ({track.Duration})\n\n");
                    trackNum++;
                }
                else
                {
                    descriptionBuilder.Append($"#{trackNum}: **[{track.Title}]** - ({track.Duration})\n\n");
                    trackNum++;
                }
            }

            
            var embed = new EmbedBuilder
            {
                Title = $"Queue",
                ThumbnailUrl = artwork,
                Description = $"Now Playing: [{player.Track.Title}]({player.Track.Duration})\n\n{descriptionBuilder}",
                Color = Global.NayuColor
            };

            return embed.Build();
        }

        public async Task<Embed> NowPlayingAsync(ShardedCommandContext context)
        {
            if (!_lavaNode.TryGetPlayer(context.Guild, out var player))
            {
                return EmbedHandler.CreateEmbed(context, "Music",
                    $"{Global.ENo} | I'm not connected to a voice channel.", EmbedHandler.EmbedMessageType.Error);
            }

            if (player.PlayerState != PlayerState.Playing)
            {
                return EmbedHandler.CreateEmbed(context, "Music",
                    $"{Global.ENo} | There's nothing playing at the moment", EmbedHandler.EmbedMessageType.Error);
            }

            var track = player.Track;
            var artwork = await track.FetchArtworkAsync();

            var embed = new EmbedBuilder
            {
                Title = $"{track.Author} - {track.Title}",
                ThumbnailUrl = artwork,
                Description = GetSong(player),
                Url = track.Url,
                Color = Global.NayuColor,
            };
            embed.WithFooter($"Id: {track.Id}");

            return embed.Build();
        }

        private static string GetSong(LavaPlayer player)
        {
            var track = player.Track;

            var formattedDuration = track.Duration.ToString(track.Duration.Hours < 1 ? Format2 : Format1);
            var formattedPosition = track.Position.ToString(track.Position.Hours < 1 ? Format2 : Format1);
            
            var dur = track.Duration - track.Position;
            var ratio = dur / track.Duration;
            var duration = (int)Math.Floor(ratio * 37);
            var bar1 = new string('─', duration);
            var bar2 = new string('─', 37 - duration);
            var descriptionBuilder = new StringBuilder();
            var song = track.Title;
            
            descriptionBuilder.Append($"ɴᴏᴡ ᴘʟᴀʏɪɴɢ: {song}\n");
            descriptionBuilder.Append($"{bar2}⚪{bar1}\n");
            descriptionBuilder.Append(player.PlayerState == PlayerState.Playing ? "◄◄  ▐▐   ►►⠀  " : "◄◄⠀  ▶   ►►⠀  ");
            descriptionBuilder.Append($" {formattedPosition} / {formattedDuration} ");
            var volumeRatio = player.Volume / 150.0;
            var volumeNum = (int)Math.Round(volumeRatio * 5);
            var volBar1 = new string('─', volumeNum);
            var volBar2 = new string('─', 5 - volumeNum);
            var volEmoji = player.Volume >= 75 ? "🔊 " : "🔉 ";
            var volume = $" {volBar1}○{volBar2} ";
            descriptionBuilder.Append(volume);
            descriptionBuilder.Append($"{volEmoji} ⠀　　　ᴴᴰ⚙️ ❐ ⊏⊐");
            return descriptionBuilder.ToString();
        }
    }
}