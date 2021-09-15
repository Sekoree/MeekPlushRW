using DisCatSharp.CommandsNext;
using DisCatSharp.Entities;
using DisCatSharp.Interactivity.Extensions;
using DisCatSharp.Lavalink;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeekPlush
{
    class Utilities
    {
        public static async Task<List<LavalinkTrack>> GetTrack(CommandContext ctx, string song)
        {
            var inter = ctx.Client.GetInteractivity();
            if (!song.StartsWith("https://") && !song.StartsWith("http://"))
            {
                var Tracks = await Bot.LLEU.GetGuildConnection(ctx.Guild).GetTracksAsync(song);
                if (Tracks.LoadResultType == LavalinkLoadResultType.LoadFailed
                    || Tracks.LoadResultType == LavalinkLoadResultType.NoMatches)
                {
                    return null;
                }
                var emb = new DiscordEmbedBuilder();
                emb.WithColor(new DiscordColor("6785A9"));
                emb.WithTitle("Select A Track!");
                string tracks = "**Respond with the number of the Track you want to select! (or type 'cancel' to cancel this)**";
                string[] nums = new[] { "1", "2", "3", "4", "5" };
                int am = 5;
                if (Tracks.Tracks.Count() < 5) am = Tracks.Tracks.Count();
                for (int i = 1; i <= am; i++)
                {
                    var cur = Tracks.Tracks.ElementAt(i - 1);
                    string time = "";
                    if (cur.Length.Hours < 1) time = cur.Length.ToString(@"mm\:ss");
                    else time = cur.Length.ToString(@"hh\:mm\:ss");
                    emb.AddField($"{nums[i - 1]}. {cur.Title} [{time}]",$"by {cur.Author}");
                }
                emb.WithDescription(tracks);
                var res1 = await ctx.RespondAsync(embed: emb.Build());
                var selTrack = await inter.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id
                && (x.Content.StartsWith("1")
                || x.Content.StartsWith("2")
                || x.Content.StartsWith("3")
                || x.Content.StartsWith("4")
                || x.Content.StartsWith("5")
                || x.Content.StartsWith("cancel")), TimeSpan.FromMinutes(1));
                Console.WriteLine(selTrack.Result.Content);
                if (selTrack.Result.Content.StartsWith("1"))
                {
                    await selTrack.Result.DeleteAsync();
                    await res1.DeleteAsync();
                    return new List<LavalinkTrack> { Tracks.Tracks.ElementAt(0) };
                    //return new List<LavalinkTrack> { new LavalinkTrack { TrackString = Tracks.Tracks.ElementAt(0).TrackString } };
                }
                else if (selTrack.Result.Content.StartsWith("2"))
                {
                    await selTrack.Result.DeleteAsync();
                    await res1.DeleteAsync();
                    return new List<LavalinkTrack> { Tracks.Tracks.ElementAt(1) };
                }
                else if (selTrack.Result.Content.StartsWith("3"))
                {
                    await selTrack.Result.DeleteAsync();
                    await res1.DeleteAsync();
                    return new List<LavalinkTrack> { Tracks.Tracks.ElementAt(2) };
                }
                else if (selTrack.Result.Content.StartsWith("4"))
                {
                    await selTrack.Result.DeleteAsync();
                    await res1.DeleteAsync();
                    return new List<LavalinkTrack> { Tracks.Tracks.ElementAt(3) };
                }
                else if (selTrack.Result.Content.StartsWith("5"))
                {
                    await selTrack.Result.DeleteAsync();
                    await res1.DeleteAsync();
                    return new List<LavalinkTrack> { Tracks.Tracks.ElementAt(4) };
                }
                else if (selTrack.Result.Content.StartsWith("cancel"))
                {
                    await ctx.Message.DeleteAsync();
                    await res1.DeleteAsync();
                    return null;
                }
                else return null;
            }
            else
            {
                var Tracks = await Bot.LLEU.GetGuildConnection(ctx.Guild).GetTracksAsync(new Uri(song));
                if (Tracks.LoadResultType == LavalinkLoadResultType.LoadFailed
                    || Tracks.LoadResultType == LavalinkLoadResultType.NoMatches)
                {
                    return null;
                }
                if (Tracks.LoadResultType == LavalinkLoadResultType.PlaylistLoaded)
                {
                    Console.WriteLine(Tracks.Tracks.Count());
                    var emb = new DiscordEmbedBuilder();
                    emb.WithColor(new DiscordColor("6785A9"));
                    emb.WithTitle("Playlist link detected");
                    string plchoose = "Do you qant to add the entire Playlist\n" +
                            "type ``yes`` or ``1`` to add the **__entire__** Playlist\n" +
                            "type ``cancel`` to cancel this";
                    if (Tracks.PlaylistInfo.SelectedTrack != -1)
                    {
                        plchoose = "Do you qant to add the entire Playlist or just the Song its referring to?\n" +
                            "type ``yes`` or ``1`` to add just that __**one**__ Song\n" +
                            "type ``no`` or ``2`` to add the **__entire__** Playlist\n" +
                            "type ``cancel`` to cancel this";
                    }
                    emb.WithDescription(plchoose);
                    var res1 = await ctx.RespondAsync(embed: emb.Build());
                    var selTrack = await inter.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id
                    && (x.Content.StartsWith("yes")
                    || x.Content.StartsWith("1")
                    || x.Content.StartsWith("no")
                    || x.Content.StartsWith("2")
                    || x.Content.StartsWith("cancel")), TimeSpan.FromMinutes(1));
                    if ((selTrack.Result.Content.StartsWith("yes") || selTrack.Result.Content.StartsWith("1")) && Tracks.PlaylistInfo.SelectedTrack != -1)
                    {
                        await selTrack.Result.DeleteAsync();
                        await res1.DeleteAsync();
                        return new List<LavalinkTrack> { Tracks.Tracks.ElementAt(Tracks.PlaylistInfo.SelectedTrack) };
                    }
                    else if ((selTrack.Result.Content.StartsWith("yes") || selTrack.Result.Content.StartsWith("1")) && Tracks.PlaylistInfo.SelectedTrack == -1)
                    {
                        await selTrack.Result.DeleteAsync();
                        await res1.DeleteAsync();
                        return new List<LavalinkTrack>(Tracks.Tracks);
                    }
                    else if ((selTrack.Result.Content.StartsWith("no") || selTrack.Result.Content.StartsWith("2")) && Tracks.PlaylistInfo.SelectedTrack != -1)
                    {
                        await selTrack.Result.DeleteAsync();
                        await res1.DeleteAsync();
                        return new List<LavalinkTrack>(Tracks.Tracks);
                    }
                    else if (selTrack.Result.Content.StartsWith("cancel"))
                    {
                        await ctx.Message.DeleteAsync();
                        await res1.DeleteAsync();
                        return null;
                    }
                    else return null;
                }
                else return new List<LavalinkTrack> { Tracks.Tracks.First() };
            }
        }
    }
}
