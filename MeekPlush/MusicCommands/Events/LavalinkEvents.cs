using DisCatSharp.Lavalink;
using DisCatSharp.Lavalink.EventArgs;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace MeekPlush.Events.MusicCommands
{
    class LavalinkEvents
    {
        public static async Task TrackEnd(LavalinkGuildConnection c, TrackFinishEventArgs lg)
        {
            Console.WriteLine(lg.Reason);
            Console.WriteLine(lg.Track.IsStream);
            if (lg.Reason == TrackEndReason.Finished && lg.Player.CurrentState.CurrentTrack.IsStream)
            {
                await Bot.Guilds[lg.Player.Guild.Id].GuildConnection.PlayAsync(Bot.Guilds[lg.Player.Guild.Id].CurrentSong.Track);
            }
            else if (lg.Reason == TrackEndReason.Finished && !Bot.Guilds[lg.Player.Guild.Id].Stop
                || lg.Reason == TrackEndReason.LoadFailed && !Bot.Guilds[lg.Player.Guild.Id].Stop)
            {
                if (lg.Reason == TrackEndReason.LoadFailed)
                {
                    await lg.Player.Guild.GetChannel(Bot.Guilds[lg.Player.Guild.Id].UsedChannel).SendMessageAsync("Track failed to load, so it got skipped");
                }
                if (!Bot.Guilds[lg.Player.Guild.Id].Repeat && !Bot.Guilds[lg.Player.Guild.Id].RepeatAll && Bot.Guilds[lg.Player.Guild.Id].GuildConnection != null)
                {
                    try
                    {
                        Bot.Guilds[lg.Player.Guild.Id].Queue.Remove(Bot.Guilds[lg.Player.Guild.Id].Queue.First(x => x.RequestTime == Bot.Guilds[lg.Player.Guild.Id].CurrentSong.RequestTime));
                    }
                    catch { }
                }
                if (Bot.Guilds[lg.Player.Guild.Id].Queue.Count > 0 && Bot.Guilds[lg.Player.Guild.Id].GuildConnection?.IsConnected == true)
                {
                    int nextSong = 0;
                    Random rnd = new Random();
                    if (Bot.Guilds[lg.Player.Guild.Id].RepeatAll)
                    {
                        Bot.Guilds[lg.Player.Guild.Id].RepeatAllPosition++; nextSong = Bot.Guilds[lg.Player.Guild.Id].RepeatAllPosition;
                        if (Bot.Guilds[lg.Player.Guild.Id].RepeatAllPosition == Bot.Guilds[lg.Player.Guild.Id].Queue.Count) { Bot.Guilds[lg.Player.Guild.Id].RepeatAllPosition = 0; nextSong = 0; }
                    }
                    if (Bot.Guilds[lg.Player.Guild.Id].Shuffle) nextSong = rnd.Next(0, Bot.Guilds[lg.Player.Guild.Id].Queue.Count);
                    Bot.Guilds[lg.Player.Guild.Id].CurrentSong = Bot.Guilds[lg.Player.Guild.Id].Queue[nextSong];
                    await Bot.Guilds[lg.Player.Guild.Id].GuildConnection.PlayAsync(Bot.Guilds[lg.Player.Guild.Id].Queue[nextSong].Track);
                }
                else
                {
                    Bot.Guilds[lg.Player.Guild.Id].Playing = false;
                }
            }
            else if (lg.Reason == TrackEndReason.Stopped && Bot.Guilds[lg.Player.Guild.Id].Stop)
            {
                Bot.Guilds[lg.Player.Guild.Id].Playing = false;
                Bot.Guilds[lg.Player.Guild.Id].Stop = false;
            }
            else if (lg.Reason == TrackEndReason.Replaced)
            {
                Bot.Guilds[lg.Player.Guild.Id].Stop = false;
            }
            else
            {
                Bot.Guilds[lg.Player.Guild.Id].Playing = false;
                Bot.Guilds[lg.Player.Guild.Id].Stop = false;
            }
            await Task.FromResult(true);
        }
    }
}
