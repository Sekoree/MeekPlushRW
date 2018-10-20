using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using System;
using System.Collections.Generic;
using System.Text;

namespace MeekPlush
{
    public class GuildList
    {
        public string GuildName { get; set; }
        public string Prefix { get; set; }
        public bool Repeat { get; set; }
        public bool RepeatAll { get; set; }
        public int RepeatAllPosition { get; set; }
        public bool Shuffle { get; set; }
        public bool Playing { get; set; }
        public bool Stop { get; set; }
        public bool Paused { get; set; }
        public bool Alone { get; set; }
        public ulong UsedChannel { get; set; }
        public QueueBase CurrentSong { get; set; }
        public LavalinkGuildConnection GuildConnection { get; set; }
        public List<QueueBase> Queue { get; set; }
    }

    public class QueueBase
    {
        public DiscordMember Requester { get; set; }
        public LavalinkTrack Track { get; set; }
        public DateTime RequestTime { get; set; }
    }

    public class MemberList
    {
        public string Prefix { get; set; }
        public Dictionary<string, Playlists> Playlists = new Dictionary<string, Playlists>();
    }

    public class Playlists
    {
        public List<PlaylistEntrys> Entries = new List<PlaylistEntrys>();
        public bool Public { get; set; }
    }

    public class PlaylistEntrys
    {
        public string URL { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
    }
}
