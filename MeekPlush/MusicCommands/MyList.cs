using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Lavalink;
using MeekPlush.Events.MusicCommands;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeekPlush.MusicCommands
{
    [Group("mylist"), Aliases("list", "ml")]
    class MyList : BaseCommandModule
    {
        [GroupCommand]
        public async Task ListMain(CommandContext ctx)
        {
            var emb = new DiscordEmbedBuilder();
            emb.WithTitle("MeekPlush Help");
            string prefix = "m!";
            if (Bot.Guilds[ctx.Guild.Id].Prefix != "m!" && Bot.Members[ctx.Member.Id].Prefix != "m!") prefix = Bot.Members[ctx.Member.Id].Prefix;
            else if (Bot.Guilds[ctx.Guild.Id].Prefix != "m!" && Bot.Members[ctx.Member.Id].Prefix == "m!") prefix = Bot.Guilds[ctx.Guild.Id].Prefix;
            emb.WithThumbnailUrl(ctx.Client.CurrentUser.AvatarUrl);
            emb.WithDescription($"__**Playlist Commands**__\n\n" +
                $"**{prefix}ml create <PlaylistName>** || Creates a Playlist with that name\n" +
                $"**{prefix}ml savequeue <PlaylistName>** || Creates a Playlist with that name and saves the current Queue to it\n" +
                $"**{prefix}ml delete <PlaylistName>** || Deletes that Playlist\n" +
                $"**{prefix}ml list** || Lists all your Playlists\n" +
                $"**{prefix}ml add <SongNameOrURL>** || Add a song to a playlist u can also 'import' YT playlists with this\n" +
                $"**{prefix}ml insert <SongNameOrURL>** || Insert A song or YT playlist at a certain position int one of your Playlists\n" +
                $"**{prefix}ml remove <Position>** || Removes a song from a Playlists at a certain Position (refer to '{prefix}ml show')\n" +
                $"**{prefix}ml rename (<OldPlaylistName>)** || Rename a Playlist if no playlist name is provided you will be prompted to do so\n" +
                $"**{prefix}ml queue (<PlaylistName>)** || Add the Songs of a Playlist to the Queue (if no name is provided you will be prompted)\n" +
                $"**{prefix}ml play (<PlaylistName>)** || Add a Playlist to the Queue and play it (if no name is provided you will be prompted)" +
                $"**{prefix}ml setaccess (<PlaylistName>)** || Make one of your playlist either public or private (if no name is provided you will be prompted)\n" +
                $"**{prefix}ml copy (<NewPlaylistName>)** || Make a copy of a playlist (if no name is provided you will be prompted)\n" +
                $"**{prefix}ml show (<PlaylistName>)** || show the songs of a playlist (if no name is provided you will be prompted)");
            emb.AddField("GuildInfo", $"Your prefix is: {Bot.Members[ctx.Member.Id].Prefix}\n" +
    $"This Guilds Prefix is: {Bot.Guilds[ctx.Guild.Id].Prefix}");
            emb.AddField("Info", $"Avatar by Chillow#1945 :heart: [Twitter](https://twitter.com/SaikoSamurai)\n" +
                $"Bot Github: soon™\n" +
                $"DBL: [Link](https://discordbots.org/bot/465675368775417856) every upvote helps and is appreciated uwu\n" +
                $"Support Server: [Invite](https://discord.gg/YPPA2Pu)" +
                $"[Paypal](https://www.paypal.me/speyd3r) [Patreon](https://www.patreon.com/speyd3r)");
            emb.WithColor(new DiscordColor("6785A9"));
            await ctx.RespondAsync(embed: emb.Build());
        }

        [Command("create")]
        public async Task PlCreate(CommandContext ctx, [RemainingText]string name = null)
        {
            if (name == null) return;
            if (Bot.Members[ctx.Member.Id].Playlists.Any(x => x.Key == name))
            {
                await ctx.RespondAsync("This Playlist Already Exists");
                return;
            }
            Bot.Members[ctx.Member.Id].Playlists.Add(name, new Playlists());
            try
            {
                var DBCon = new MySqlConnection();
                await DBCon.OpenAsync();
                MySqlCommand addGuild = new MySqlCommand
                {
                    Connection = DBCon,
                    CommandText = $"INSERT INTO `playlists` (`UserID`, `PlaylistName`, `Public`) VALUES ('{ctx.Member.Id.ToString()}', @list, 0) ON DUPLICATE KEY UPDATE PlaylistName=PlaylistName"
                };
                addGuild.Parameters.AddWithValue("@list", name);
                await addGuild.ExecuteNonQueryAsync();
                await DBCon.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            await ctx.RespondAsync("Created Playlist: " + name);
        }

        [Command("savequeue")]
        public async Task PlSaveQueue(CommandContext ctx, [RemainingText]string name = null)
        {
            if (name == null) return;
            if (Bot.Members[ctx.Member.Id].Playlists.Any(x => x.Key == name))
            {
                await ctx.RespondAsync("This Playlist Already Exists");
                return;
            }
            if (Bot.Guilds[ctx.Guild.Id].Queue.Count == 0)
            {
                await ctx.RespondAsync("Queue Empty");
                return;
            }
            Bot.Members[ctx.Member.Id].Playlists.Add(name, new Playlists());
            try
            {
                var DBCon = new MySqlConnection();
                await DBCon.OpenAsync();
                MySqlCommand addGuild = new MySqlCommand
                {
                    Connection = DBCon,
                    CommandText = $"INSERT INTO `playlists` (`UserID`, `PlaylistName`, `Public`) VALUES ('{ctx.Member.Id.ToString()}', @list, 0) ON DUPLICATE KEY UPDATE PlaylistName=PlaylistName"
                };
                addGuild.Parameters.AddWithValue("@list", name);
                await addGuild.ExecuteNonQueryAsync();
                await DBCon.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            foreach(var Track in Bot.Guilds[ctx.Guild.Id].Queue)
            {
                int c = Bot.Members[ctx.Member.Id].Playlists[name].Entries.Count;
                Bot.Members[ctx.Member.Id].Playlists[name].Entries.Add(new PlaylistEntrys
                {
                    Author = Track.Track.Author,
                    Title = Track.Track.Title,
                    URL = Track.Track.Uri.OriginalString
                });
                try
                {
                    var DBCon = new MySqlConnection();
                    await DBCon.OpenAsync();
                    MySqlCommand addGuild = new MySqlCommand
                    {
                        Connection = DBCon,
                        CommandText = $"INSERT INTO `playlistEntries` (`UserID`, `PlaylistName`, `URL`, `Author`, `Title`, `Position`)" +
                        $" VALUES ('{ctx.Member.Id}'," +
                        $" @list," +
                        $" @url," +
                        $" @author," +
                        $" @title," +
                        $" '{Bot.Members[ctx.Member.Id].Playlists[name].Entries.Count - 1}') ON DUPLICATE KEY UPDATE PlaylistName=PlaylistName"
                    };
                    addGuild.Parameters.AddWithValue("@list", name);
                    addGuild.Parameters.AddWithValue("@url", Track.Track.Uri.AbsoluteUri);
                    addGuild.Parameters.AddWithValue("@author", Track.Track.Author);
                    addGuild.Parameters.AddWithValue("@title", Track.Track.Title);
                    await addGuild.ExecuteNonQueryAsync();
                    await DBCon.CloseAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            await ctx.RespondAsync("Created Playlist from Queue: " + name);
        }

        [Command("delete")]
        public async Task PlDelete(CommandContext ctx, [RemainingText]string name = null)
        {
            if (name == null) return;
            if (!Bot.Members[ctx.Member.Id].Playlists.Any(x => x.Key == name))
            {
                await ctx.RespondAsync("This Playlist Does Not Exist");
                return;
            }
            Bot.Members[ctx.Member.Id].Playlists.Remove(name);
            try
            {
                var DBCon = new MySqlConnection();
                await DBCon.OpenAsync();
                MySqlCommand addGuild = new MySqlCommand
                {
                    Connection = DBCon,
                    CommandText = $"DELETE FROM `playlists` WHERE `playlists`.`UserID` = {ctx.Member.Id.ToString()} AND `playlists`.`PlaylistName` = \'{name}\'"
                };
                await addGuild.ExecuteNonQueryAsync();
                await DBCon.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            try
            {
                var DBCon = new MySqlConnection();
                await DBCon.OpenAsync();
                MySqlCommand addGuild = new MySqlCommand
                {
                    Connection = DBCon,
                    CommandText = $"DELETE FROM `playlistEntries` WHERE `playlistEntries`.`UserID` = {ctx.Member.Id.ToString()} AND `playlistEntries`.`PlaylistName` = \'{name}\'"
                };
                await addGuild.ExecuteNonQueryAsync();
                await DBCon.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            await ctx.RespondAsync("Deleted Playlist: " + name);
        }

        [Command("list")]
        public async Task PlList(CommandContext ctx)
        {
            var inter = ctx.Client.GetInteractivity();
            string Pls = "";
            int k = 1;
            var emb = new DiscordEmbedBuilder();
            emb.WithColor(new DiscordColor("6785A9"));
            emb.WithTitle($"Your Playlists");
            List<Page> pages = new List<Page>();
            foreach (var pls in Bot.Members[ctx.Member.Id].Playlists)
            {
                Pls += k + ". " + pls.Key + $" ({pls.Value.Entries.Count} Songs)\n";
                k++;
            }
            emb.WithDescription(Pls);
            pages.Add(new Page {
                Embed = emb.Build()
            });
            emb.WithTitle($"Public Playlists");
            int i = 0;
            Pls = "";
            foreach (var PPls in Bot.Members.Where(x => x.Value.Playlists.Any(y => y.Value.Public) && x.Key != ctx.Member.Id))
            {
                var User = await ctx.Client.GetUserAsync(PPls.Key);
                foreach (var BoiPls in PPls.Value.Playlists.Where(x => x.Value.Public))
                {
                    Pls += $"{BoiPls.Key} [{BoiPls.Value.Entries.Count}]\n" +
                        $"by {User.Username}#{User.Discriminator}\n\n";
                    i++;
                    if (i == 5)
                    {
                        i = 0;
                        emb.WithDescription(Pls);
                        Pls = "";
                        pages.Add(new Page {
                            Embed = emb.Build()
                        });
                    }
                }
            }
            if (i != 5 && Pls.Length != 0)
            {
                emb.WithDescription(Pls);
                pages.Add(new Page
                {
                    Embed = emb.Build()
                });
            }
            await inter.SendPaginatedMessage(ctx.Channel, ctx.User, pages, TimeSpan.FromMinutes(2.5), TimeoutBehaviour.DeleteReactions);
        }

        [Command("add")]
        public async Task PlAddSong(CommandContext ctx, [RemainingText]string song = null)
        {
            string selectedList = "";
            if (!Bot.Members[ctx.Member.Id].Playlists.Any() || song == null)
            {
                return;
            }
            var inter = ctx.Client.GetInteractivity();
            int i = 1;
            var PList = Bot.Members[ctx.Member.Id].Playlists.ToList();
            string Pls = "Type the Playlistname of the Number to add Songs to:\n\n";
            foreach (var pls in Bot.Members[ctx.Member.Id].Playlists)
            {
                Pls += i + ". " + pls.Key + $" ({pls.Value.Entries.Count} Songs)\n";
                i++;
            }
            var Plist = await ctx.RespondAsync(Pls);
            var pl = await inter.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id 
            && (Bot.Members[ctx.Member.Id].Playlists.Any(y => y.Key == x.Content)
            || PList[Convert.ToInt32(x.Content) - 1].Key != null
            || x.Content.StartsWith("cancel")), TimeSpan.FromSeconds(30));
            if (Bot.Members[ctx.Member.Id].Playlists.Any(x => x.Key == pl.Message.Content))
            {
                selectedList = pl.Message.Content;
                await pl.Message.DeleteAsync();
                await Plist.DeleteAsync();
            }
            else if (PList[Convert.ToInt32(pl.Message.Content) - 1].Key != null)
            {
                selectedList = PList[Convert.ToInt32(pl.Message.Content) - 1].Key;
                await pl.Message.DeleteAsync();
                await Plist.DeleteAsync();
            }
            else
            {
                await pl.Message.DeleteAsync();
                await Plist.DeleteAsync();
                return;
            }
            var Get = await Utilities.GetTrack(ctx, song);
            var Tracks = Get;
            if (Tracks == null) return;
            foreach (var Track in Tracks)
            {
                int c = Bot.Members[ctx.Member.Id].Playlists[selectedList].Entries.Count;
                Bot.Members[ctx.Member.Id].Playlists[selectedList].Entries.Add(new PlaylistEntrys
                {
                    Author = Track.Author,
                    Title = Track.Title,
                    URL = Track.Uri.OriginalString
                });
                try
                {
                    var DBCon = new MySqlConnection();
                    await DBCon.OpenAsync();
                    MySqlCommand addGuild = new MySqlCommand
                    {
                        Connection = DBCon,
                        CommandText = $"INSERT INTO `playlistEntries` (`UserID`, `PlaylistName`, `URL`, `Author`, `Title`, `Position`)" +
                        $" VALUES ('{ctx.Member.Id}'," +
                        $" ?," +
                        $" ?," +
                        $" ?," +
                        $" ?," +
                        $" '{Bot.Members[ctx.Member.Id].Playlists[selectedList].Entries.Count - 1}') ON DUPLICATE KEY UPDATE PlaylistName=PlaylistName"
                    };
                    addGuild.Parameters.Add("PlaylistName", MySqlDbType.VarChar).Value = selectedList;
                    addGuild.Parameters.Add("URL", MySqlDbType.VarChar).Value = Track.Uri.AbsoluteUri;
                    addGuild.Parameters.Add("Author", MySqlDbType.VarChar).Value = Track.Author;
                    addGuild.Parameters.Add("Title", MySqlDbType.VarChar).Value = Track.Title;
                    await addGuild.ExecuteNonQueryAsync();
                    await DBCon.CloseAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            if (Tracks.Count == 1)
            {
                await ctx.RespondAsync($"Added **{Tracks.First().Title}** by **{Tracks.First().Author}** to **{selectedList}**");
            }
            else
            {
                await ctx.RespondAsync($"Added {Tracks.Count} Songs to {selectedList}");
            }
        }

        [Command("copy")]
        public async Task PlCopyList(CommandContext ctx, [RemainingText]string name = null)
        {
            Playlists selectedList = new Playlists();
            if (!Bot.Members[ctx.Member.Id].Playlists.Any())
            {
                return;
            }
            var inter = ctx.Client.GetInteractivity();
            string Pls = "";
            int k = 1;
            var emb = new DiscordEmbedBuilder();
            emb.WithColor(new DiscordColor("6785A9"));
            emb.WithTitle($"Your Playlists\n" +
                $"Select the Playlist you want to copy from! Type Number or Name!");
            List<Page> pages = new List<Page>();
            Dictionary<string, Playlists> PList = new Dictionary<string, Playlists>();
            foreach (var pls in Bot.Members[ctx.Member.Id].Playlists)
            {
                Pls += k + ". " + pls.Key + $" ({pls.Value.Entries.Count} Songs)\n";
                k++;
                PList.Add(pls.Key, pls.Value);
            }
            emb.WithDescription(Pls);
            pages.Add(new Page
            {
                Embed = emb.Build()
            });
            emb.WithTitle($"Public Playlists\n" +
                $"Select the Playlist you want to copy from! Type Number or Name!"); ;
            int i = 0;
            Pls = "";
            foreach (var PPls in Bot.Members.Where(x => x.Value.Playlists.Any(y => y.Value.Public) && x.Key != ctx.Member.Id))
            {
                var User = await ctx.Client.GetUserAsync(PPls.Key);
                foreach (var BoiPls in PPls.Value.Playlists.Where(x => x.Value.Public))
                {
                    Pls += $"{k}. {BoiPls.Key} [{BoiPls.Value.Entries.Count}]\n" +
                        $"by {User.Username}#{User.Discriminator}\n\n";
                    PList.Add(BoiPls.Key, BoiPls.Value);
                    k++;
                    i++;
                    if (i == 5)
                    {
                        i = 0;
                        emb.WithDescription(Pls);
                        Pls = "";
                        pages.Add(new Page
                        {
                            Embed = emb.Build()
                        });
                    }
                }
            }
            if (i != 5 && Pls.Length != 0)
            {
                emb.WithDescription(Pls);
                pages.Add(new Page
                {
                    Embed = emb.Build()
                });
            }
            string copiedPl = "";
            inter.SendPaginatedMessage(ctx.Channel, ctx.User, pages, TimeSpan.FromMinutes(2.5), TimeoutBehaviour.DeleteMessage).Wait(0);
            var pl = await inter.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id
            && (PList.Any(y => y.Key == x.Content)
            || PList.ToList()[Convert.ToInt32(x.Content) - 1].Value != null), TimeSpan.FromSeconds(30));
            if (PList.Any(x => x.Key == pl.Message.Content))
            {
                copiedPl = PList.First(x => x.Key == pl.Message.Content).Key;
                selectedList = PList.First(x => x.Key == pl.Message.Content).Value;
                await pl.Message.DeleteAsync();
            }
            else if (PList.ToList()[Convert.ToInt32(pl.Message.Content) - 1].Value != null)
            {
                copiedPl = PList.ToList()[Convert.ToInt32(pl.Message.Content) - 1].Key;
                selectedList = PList.ToList()[Convert.ToInt32(pl.Message.Content) - 1].Value;
                await pl.Message.DeleteAsync();
            }
            else
            {
                await pl.Message.DeleteAsync();
                return;
            }
            Bot.Members[ctx.Member.Id].Playlists.Add(name, new Playlists { });
            try
            {
                var DBCon = new MySqlConnection();
                await DBCon.OpenAsync();
                MySqlCommand addGuild = new MySqlCommand
                {
                    Connection = DBCon,
                    CommandText = $"INSERT INTO `playlists` (`UserID`, `PlaylistName`, `Public`) VALUES ('{ctx.Member.Id.ToString()}', @list, {Convert.ToInt16(selectedList.Public).ToString()}) ON DUPLICATE KEY UPDATE PlaylistName=PlaylistName"
                };
                addGuild.Parameters.AddWithValue("@list", name);
                await addGuild.ExecuteNonQueryAsync();
                await DBCon.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            foreach (var Track in selectedList.Entries)
            {
                //Console.WriteLine(selectedList.Entries.Count);
                int c = Bot.Members[ctx.Member.Id].Playlists[name].Entries.Count;
                Bot.Members[ctx.Member.Id].Playlists[name].Entries.Add(new PlaylistEntrys
                {
                    Author = Track.Author,
                    Title = Track.Title,
                    URL = Track.URL
                });
                try
                {
                    var DBCon = new MySqlConnection();
                    await DBCon.OpenAsync();
                    MySqlCommand addGuild = new MySqlCommand
                    {
                        Connection = DBCon,
                        CommandText = $"INSERT INTO `playlistEntries` (`UserID`, `PlaylistName`, `URL`, `Author`, `Title`, `Position`)" +
                        $" VALUES ('{ctx.Member.Id}'," +
                        $" ?," +
                        $" ?," +
                        $" ?," +
                        $" ?," +
                        $" '{Bot.Members[ctx.Member.Id].Playlists[name].Entries.Count - 1}') ON DUPLICATE KEY UPDATE PlaylistName=PlaylistName"
                    };
                    addGuild.Parameters.Add("PlaylistName", MySqlDbType.VarChar).Value = name;
                    addGuild.Parameters.Add("URL", MySqlDbType.VarChar).Value = Track.URL;
                    addGuild.Parameters.Add("Author", MySqlDbType.VarChar).Value = Track.Author;
                    addGuild.Parameters.Add("Title", MySqlDbType.VarChar).Value = Track.Title;
                    await addGuild.ExecuteNonQueryAsync();
                    await DBCon.CloseAsync();
                }

                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            await ctx.RespondAsync($"Created copy of {copiedPl} with the name {name}!");
        }

        [Command("insert")]
        public async Task PlInsertSong(CommandContext ctx, [RemainingText]string song = null)
        {
            string selectedList = "";
            if (!Bot.Members[ctx.Member.Id].Playlists.Any() || song == null)
            {
                return;
            }
            var inter = ctx.Client.GetInteractivity();
            int k = 1;
            var PList = Bot.Members[ctx.Member.Id].Playlists.ToList();
            string Pls = "Type the Playlistname of the Number to insert Songs to:\n\n";
            foreach (var pls in Bot.Members[ctx.Member.Id].Playlists)
            {
                Pls += k + ". " + pls.Key + $" ({pls.Value.Entries.Count} Songs)\n";
                k++;
            }
            var Plist = await ctx.RespondAsync(Pls);
            var pl = await inter.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id
            && (Bot.Members[ctx.Member.Id].Playlists.Any(y => y.Key == x.Content)
            || PList[Convert.ToInt32(x.Content) - 1].Key != null), TimeSpan.FromSeconds(30));
            if (Bot.Members[ctx.Member.Id].Playlists.Any(x => x.Key == pl.Message.Content))
            {
                selectedList = pl.Message.Content;
                await pl.Message.DeleteAsync();
                await Plist.DeleteAsync();
            }
            else if (PList[Convert.ToInt32(pl.Message.Content) - 1].Key != null)
            {
                selectedList = PList[Convert.ToInt32(pl.Message.Content) - 1].Key;
                await pl.Message.DeleteAsync();
                await Plist.DeleteAsync();
            }
            else
            {
                await pl.Message.DeleteAsync();
                await Plist.DeleteAsync();
                return;
            }
            int insPos = 1;
            var Pchoose = await ctx.RespondAsync("At wich position should the track(s) be inserted?");
            var pl2 = await inter.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id
                && ((Convert.ToInt32(pl.Message.Content) - 1) <= Bot.Members[ctx.Member.Id].Playlists[selectedList].Entries.Count 
                || (Convert.ToInt32(pl.Message.Content) - 1) == -1), TimeSpan.FromSeconds(30));
            if ((Convert.ToInt32(pl2.Message.Content) - 1) <= Bot.Members[ctx.Member.Id].Playlists[selectedList].Entries.Count || (Convert.ToInt32(pl2.Message.Content) - 1) == -1)
            {
                insPos = Convert.ToInt32(pl.Message.Content) - 1;
                await pl2.Message.DeleteAsync();
                await Pchoose.DeleteAsync();
            }
            else
            {
                await pl2.Message.DeleteAsync();
                await Pchoose.DeleteAsync();
                return;
            }
            List<LavalinkTrack> Tracks = await Utilities.GetTrack(ctx, song);
            if (Tracks == null)
            {
                await ctx.RespondAsync("No track found or broken URL");
                return;
            }
            foreach (var Track in Tracks)
            {
                Bot.Members[ctx.Member.Id].Playlists[selectedList].Entries.Insert(insPos, new PlaylistEntrys
                {
                    Author = Track.Author,
                    Title = Track.Title,
                    URL = Track.Uri.OriginalString
                });
                insPos++;
            }
            try
            {
                var DBCon = new MySqlConnection();
                await DBCon.OpenAsync();
                MySqlCommand addGuild = new MySqlCommand
                {
                    Connection = DBCon,
                    CommandText = $"DELETE FROM `playlistEntries` WHERE `playlistEntries`.`UserID` = {ctx.Member.Id.ToString()} AND `playlistEntries`.`PlaylistName` = @list"
                };
                addGuild.Parameters.AddWithValue("@list", selectedList);
                await addGuild.ExecuteNonQueryAsync();
                await DBCon.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            int i = 0;
            foreach (var Track in Bot.Members[ctx.Member.Id].Playlists[selectedList].Entries)
            {
                try
                {
                    var DBCon = new MySqlConnection();
                    await DBCon.OpenAsync();
                    MySqlCommand addGuild = new MySqlCommand
                    {
                        Connection = DBCon,
                        CommandText = $"INSERT INTO `playlistEntries` (`UserID`, `PlaylistName`, `URL`, `Author`, `Title`, `Position`)" +
                        $" VALUES ('{ctx.Member.Id}'," +
                        $" @list," +
                        $" @url," +
                        $" @author," +
                        $" @title," +
                        $" '{i}') ON DUPLICATE KEY UPDATE PlaylistName=PlaylistName"
                    };
                    addGuild.Parameters.AddWithValue("@list", selectedList);
                    addGuild.Parameters.AddWithValue("@url", Track.URL);
                    addGuild.Parameters.AddWithValue("@author", Track.Author);
                    addGuild.Parameters.AddWithValue("@title", Track.Title);
                    await addGuild.ExecuteNonQueryAsync();
                    await DBCon.CloseAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                }
                i++;
            }
            if (Tracks.Count == 1)
            {
                await ctx.RespondAsync($"Inserted **{Tracks.First().Title}** by **{Tracks.First().Author}** to **{selectedList}** at Position {insPos +1}");
            }
            else
            {
                await ctx.RespondAsync($"Inserted {Tracks.Count} Songs to {selectedList} at Position {insPos+1}");
            }
        }

        [Command("remove")]
        public async Task PlRemoveSong(CommandContext ctx, int Position)
        {
            PlaylistEntrys rem = new PlaylistEntrys();
            string selectedList = "";
            if (!Bot.Members[ctx.Member.Id].Playlists.Any())
            {
                return;
            }
            int k = 1;
            var inter = ctx.Client.GetInteractivity();
            var PList = Bot.Members[ctx.Member.Id].Playlists.ToList();
            string Plsm = "Type the Playlistname of the Number to delete from:\n\n";
            foreach (var pls in Bot.Members[ctx.Member.Id].Playlists)
            {
                Plsm += k + ". " + pls.Key + $" ({pls.Value.Entries.Count} Songs)\n";
                k++;
            }
            var PlistM = await ctx.RespondAsync(Plsm);
            var pl = await inter.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id
                && (Bot.Members[ctx.Member.Id].Playlists.Any(y => y.Key == x.Content)
                || PList[Convert.ToInt32(x.Content) - 1].Key != null), TimeSpan.FromSeconds(30));
            if (Bot.Members[ctx.Member.Id].Playlists.Any(x => x.Key == pl.Message.Content))
            {
                selectedList = pl.Message.Content;
                await pl.Message.DeleteAsync();
                await PlistM.DeleteAsync();
            }
            else if (PList[Convert.ToInt32(pl.Message.Content) - 1].Key != null)
            {
                selectedList = PList[Convert.ToInt32(pl.Message.Content) - 1].Key;
                await pl.Message.DeleteAsync();
                await PlistM.DeleteAsync();
            }
            else
            {
                await pl.Message.DeleteAsync();
                await PlistM.DeleteAsync();
                return;
            }
            int c = Bot.Members[ctx.Member.Id].Playlists[selectedList].Entries.Count;
            rem = Bot.Members[ctx.Member.Id].Playlists[selectedList].Entries[Position - 1];
            Bot.Members[ctx.Member.Id].Playlists[selectedList].Entries.RemoveAt(Position - 1);
            try
            {
                var DBCon = new MySqlConnection();
                await DBCon.OpenAsync();
                MySqlCommand addGuild = new MySqlCommand
                {
                    Connection = DBCon,
                    CommandText = $"DELETE FROM `playlistEntries` WHERE `playlistEntries`.`UserID` = {ctx.Member.Id.ToString()} AND `playlistEntries`.`PlaylistName` = @list"
                };
                addGuild.Parameters.AddWithValue("@list", selectedList);
                await addGuild.ExecuteNonQueryAsync();
                await DBCon.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            int i = 0;
            foreach (var Track in Bot.Members[ctx.Member.Id].Playlists[selectedList].Entries)
            {
                try
                {
                    var DBCon = new MySqlConnection();
                    await DBCon.OpenAsync();
                    MySqlCommand addGuild = new MySqlCommand
                    {
                        Connection = DBCon,
                        CommandText = $"INSERT INTO `playlistEntries` (`UserID`, `PlaylistName`, `URL`, `Author`, `Title`, `Position`)" +
                        $" VALUES ('{ctx.Member.Id}'," +
                        $" @list," +
                        $" @url," +
                        $" @author," +
                        $" @title," +
                        $" '{i}') ON DUPLICATE KEY UPDATE PlaylistName=PlaylistName"
                    };
                    addGuild.Parameters.AddWithValue("@list", selectedList);
                    addGuild.Parameters.AddWithValue("@url", Track.URL);
                    addGuild.Parameters.AddWithValue("@author", Track.Author);
                    addGuild.Parameters.AddWithValue("@title", Track.Title);
                    await addGuild.ExecuteNonQueryAsync();
                    await DBCon.CloseAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                }
                i++;
            }
            await ctx.RespondAsync($"Removed: **{rem.Title}** by **{rem.Author}**");
        }

        [Command("rename")]
        public async Task PlRename(CommandContext ctx, [RemainingText]string name = null)
        {
            string newName = "";
            if (!Bot.Members[ctx.Member.Id].Playlists.Any())
            {
                await ctx.RespondAsync("You have no Playlists");
                return;
            }
            var inter = ctx.Client.GetInteractivity();
            if (name == null)
            {
                int i = 1;
                var PList = Bot.Members[ctx.Member.Id].Playlists.ToList();
                string Plsm = "Type the Playlistname of the Number to rename it:\n\n";
                foreach (var pls in Bot.Members[ctx.Member.Id].Playlists)
                {
                    Plsm += i + ". " + pls.Key + $" ({pls.Value.Entries.Count} Songs)\n";
                    i++;
                }
                var PlistM = await ctx.RespondAsync(Plsm);
                var pl = await inter.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id
                && (Bot.Members[ctx.Member.Id].Playlists.Any(y => y.Key == x.Content)
                || PList[Convert.ToInt32(x.Content) - 1].Key != null), TimeSpan.FromSeconds(30));
                if (Bot.Members[ctx.Member.Id].Playlists.Any(x => x.Key == pl.Message.Content))
                {
                    name = pl.Message.Content;
                    await pl.Message.DeleteAsync();
                    await PlistM.DeleteAsync();
                }
                else if (PList[Convert.ToInt32(pl.Message.Content) - 1].Key != null)
                {
                    name = PList[Convert.ToInt32(pl.Message.Content) - 1].Key;
                    await pl.Message.DeleteAsync();
                    await PlistM.DeleteAsync();
                }
                else
                {
                    await pl.Message.DeleteAsync();
                    await PlistM.DeleteAsync();
                    return;
                }
            }
            string Pls = $"Type the new Playlistname for {name}";
            var Plist = await ctx.RespondAsync(Pls);
            var pl2 = await inter.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id
            && x.Content.Length != 0, TimeSpan.FromSeconds(30));
            if (Bot.Members[ctx.Member.Id].Playlists.Any(x => x.Key == pl2.Message.Content))
            {
                await ctx.RespondAsync("You already havea Playlist with that name! canceled uwu");
                return;
            }
            if (pl2.Message.Content.Length != 0)
            {
                newName = pl2.Message.Content;
                await pl2.Message.DeleteAsync();
                await Plist.DeleteAsync();
            }
            else
            {
                await pl2.Message.DeleteAsync();
                await Plist.DeleteAsync();
                return;
            }
            try
            {
                var DBCon = new MySqlConnection();
                await DBCon.OpenAsync();
                MySqlCommand addGuild = new MySqlCommand
                {
                    Connection = DBCon,
                    CommandText = $"UPDATE `playlists` SET `PlaylistName` = @newName WHERE `playlists`.`UserID` = {ctx.Member.Id.ToString()} AND `playlists`.`PlaylistName` = @oldName;"
                };
                addGuild.Parameters.AddWithValue("@newName", newName);
                addGuild.Parameters.AddWithValue("@oldName", name);
                await addGuild.ExecuteNonQueryAsync();
                await DBCon.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            try
            {
                var DBCon = new MySqlConnection();
                await DBCon.OpenAsync();
                MySqlCommand addGuild = new MySqlCommand
                {
                    Connection = DBCon,
                    CommandText = $"UPDATE `playlistEntries` SET `PlaylistName` = @newName WHERE `playlistEntries`.`PlaylistName` = @oldName AND `playlistEntries`.`UserID` = {ctx.Member.Id.ToString()};"
                };
                addGuild.Parameters.AddWithValue("@newName", newName);
                addGuild.Parameters.AddWithValue("@oldName", name);
                await addGuild.ExecuteNonQueryAsync();
                await DBCon.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            var old = Bot.Members[ctx.Member.Id].Playlists[name];
            Bot.Members[ctx.Member.Id].Playlists.Remove(name);
            Bot.Members[ctx.Member.Id].Playlists.Add(newName, old);
            await ctx.RespondAsync("Renamed playlist to: " + newName);
        }

        [Command("setaccess")]
        public async Task PlmakePublic(CommandContext ctx, [RemainingText]string name = null)
        {
            if (!Bot.Members[ctx.Member.Id].Playlists.Any())
            {
                await ctx.RespondAsync("You have no Playlists");
                return;
            }
            var inter = ctx.Client.GetInteractivity();
            if (name == null)
            {
                int i = 1;
                var PList = Bot.Members[ctx.Member.Id].Playlists.ToList();
                string Plsm = "Type the Playlistname of the Number to set its accessability:\n\n";
                foreach (var pls in Bot.Members[ctx.Member.Id].Playlists)
                {
                    Plsm += i + ". " + pls.Key + $" ({pls.Value.Entries.Count} Songs)\n";
                    i++;
                }
                var PlistM = await ctx.RespondAsync(Plsm);
                var pl = await inter.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id
                && (Bot.Members[ctx.Member.Id].Playlists.Any(y => y.Key == x.Content)
                || PList[Convert.ToInt32(x.Content) - 1].Key != null), TimeSpan.FromSeconds(30));
                if (Bot.Members[ctx.Member.Id].Playlists.Any(x => x.Key == pl.Message.Content))
                {
                    name = pl.Message.Content;
                    await pl.Message.DeleteAsync();
                    await PlistM.DeleteAsync();
                }
                else if (PList[Convert.ToInt32(pl.Message.Content) - 1].Key != null)
                {
                    name = PList[Convert.ToInt32(pl.Message.Content) - 1].Key;
                    await pl.Message.DeleteAsync();
                    await PlistM.DeleteAsync();
                }
                else
                {
                    await pl.Message.DeleteAsync();
                    await PlistM.DeleteAsync();
                    return;
                }
            }
            try
            {
                int acc = Convert.ToInt32(Bot.Members[ctx.Member.Id].Playlists[name].Public);
                if (acc == 1)
                {
                    acc = 0;
                    Bot.Members[ctx.Member.Id].Playlists[name].Public = false;
                }
                else
                {
                    acc = 1;
                    Bot.Members[ctx.Member.Id].Playlists[name].Public = true;
                }
                var DBCon = new MySqlConnection();
                await DBCon.OpenAsync();
                MySqlCommand addGuild = new MySqlCommand
                {
                    Connection = DBCon,
                    CommandText = $"UPDATE `playlists` SET `Public` = @newName WHERE `playlists`.`UserID` = {ctx.Member.Id.ToString()} AND `playlists`.`PlaylistName` = @oldName;"
                };
                addGuild.Parameters.AddWithValue("@newName", acc);
                addGuild.Parameters.AddWithValue("@oldName", name);
                await addGuild.ExecuteNonQueryAsync();
                await DBCon.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            if (Bot.Members[ctx.Member.Id].Playlists[name].Public)
            {
                await ctx.RespondAsync($"{name} is now public!");
            }
            else
            {
                await ctx.RespondAsync($"{name} is now private!");
            }
        }

        [Command("queue")]
        public async Task PlQueueLoad(CommandContext ctx, [RemainingText]string list = null)
        {
            if (!Bot.Members[ctx.Member.Id].Playlists.Any())
            {
                return;
            }
            var inter = ctx.Client.GetInteractivity();
            Playlists selectedList = new Playlists();
            string PlName = "";
            if (list == null)
            {
                string Pls = "";
                int k = 1;
                var emb2 = new DiscordEmbedBuilder();
                emb2.WithColor(new DiscordColor("6785A9"));
                emb2.WithTitle($"Your Playlists");
                List<Page> pages = new List<Page>();
                Dictionary<string, Playlists> PList = new Dictionary<string, Playlists>();
                foreach (var pls in Bot.Members[ctx.Member.Id].Playlists)
                {
                    Pls += k + ". " + pls.Key + $" ({pls.Value.Entries.Count} Songs)\n";
                    k++;
                    PList.Add(pls.Key, pls.Value);
                }
                emb2.WithDescription(Pls);
                pages.Add(new Page
                {
                    Embed = emb2.Build()
                });
                emb2.WithTitle($"Public Playlists");
                int ii = 0;
                Pls = "";
                foreach (var PPls in Bot.Members.Where(x => x.Value.Playlists.Any(y => y.Value.Public) && x.Key != ctx.Member.Id))
                {
                    var User = await ctx.Client.GetUserAsync(PPls.Key);
                    foreach (var BoiPls in PPls.Value.Playlists.Where(x => x.Value.Public))
                    {
                        Pls += $"{k}. {BoiPls.Key} [{BoiPls.Value.Entries.Count}]\n" +
                            $"by {User.Username}#{User.Discriminator}\n\n";
                        PList.Add(BoiPls.Key, BoiPls.Value);
                        k++;
                        ii++;
                        if (ii == 5)
                        {
                            ii = 0;
                            emb2.WithDescription(Pls);
                            Pls = "";
                            pages.Add(new Page
                            {
                                Embed = emb2.Build()
                            });
                        }
                    }
                }
                if (ii != 5 && Pls.Length != 0)
                {
                    emb2.WithDescription(Pls);
                    pages.Add(new Page
                    {
                        Embed = emb2.Build()
                    });
                }
                inter.SendPaginatedMessage(ctx.Channel, ctx.User, pages, TimeSpan.FromMinutes(2.5), TimeoutBehaviour.DeleteMessage).Wait(0);
                var pl = await inter.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id
                && (Bot.Members[ctx.Member.Id].Playlists.Any(y => y.Key == x.Content
                || PList.ToList()[Convert.ToInt32(x.Content) - 1].Value != null)), TimeSpan.FromSeconds(30));
                if (Bot.Members[ctx.Member.Id].Playlists.Any(x => x.Key == pl.Message.Content))
                {
                    PlName = PList.ToList().First(x => x.Key == pl.Message.Content).Key;
                    selectedList = PList.ToList().First(x => x.Key == pl.Message.Content).Value;
                    await pl.Message.DeleteAsync();
                }
                else if (PList.ToList()[Convert.ToInt32(pl.Message.Content) - 1].Value != null)
                {
                    PlName = PList.ToList()[Convert.ToInt32(pl.Message.Content) - 1].Key;
                    selectedList = PList.ToList()[Convert.ToInt32(pl.Message.Content) - 1].Value;
                    await pl.Message.DeleteAsync();
                }
                else
                {
                    await pl.Message.DeleteAsync();
                    return;
                }
            }
            else
            {
                selectedList = Bot.Members[ctx.Member.Id].Playlists[list];
            }
            await ctx.RespondAsync("Depending on playlist size this can take a while, there will be another message when its done!");
            int j = Bot.Members[ctx.Member.Id].Playlists[list].Entries.Count;
            string failedSongs = "These songs coulnt be added since they were either regionblocked or had broken Links:\n\n";
            foreach (var PlE in selectedList.Entries)
            {
                var Get = await Utilities.GetTrack(ctx, PlE.URL);
                if (Get == null)
                {
                    failedSongs += $"{PlE.Title}\n" +
                        $"by {PlE.Author} [Link]({PlE.URL})";
                    j--;
                    continue;
                }
                Bot.Guilds[ctx.Guild.Id].Queue.Add(new QueueBase {
                    Requester = ctx.Member,
                    RequestTime = DateTime.Now,
                    Track = Get[0]
            });
            }
            await ctx.RespondAsync($"added {j} to queue");
            if (j != selectedList.Entries.Count)
            {
                await ctx.RespondAsync(failedSongs);
            }
        }

        [Command("play")]
        public async Task PlQueuePlay(CommandContext ctx, [RemainingText]string list = null)
        {
            if (!Bot.Members[ctx.Member.Id].Playlists.Any())
            {
                return;
            }
            var inter = ctx.Client.GetInteractivity();
            Playlists selectedList = new Playlists();
            string PlName = "";
            if (list == null)
            {
                string Pls = "";
                int k = 1;
                var emb2 = new DiscordEmbedBuilder();
                emb2.WithColor(new DiscordColor("6785A9"));
                emb2.WithTitle($"Your Playlists");
                List<Page> pages = new List<Page>();
                Dictionary<string, Playlists> PList = new Dictionary<string, Playlists>();
                foreach (var pls in Bot.Members[ctx.Member.Id].Playlists)
                {
                    Pls += k + ". " + pls.Key + $" ({pls.Value.Entries.Count} Songs)\n";
                    k++;
                    PList.Add(pls.Key, pls.Value);
                }
                emb2.WithDescription(Pls);
                pages.Add(new Page
                {
                    Embed = emb2.Build()
                });
                emb2.WithTitle($"Public Playlists");
                int ii = 0;
                Pls = "";
                foreach (var PPls in Bot.Members.Where(x => x.Value.Playlists.Any(y => y.Value.Public) && x.Key != ctx.Member.Id))
                {
                    var User = await ctx.Client.GetUserAsync(PPls.Key);
                    foreach (var BoiPls in PPls.Value.Playlists.Where(x => x.Value.Public))
                    {
                        Pls += $"{k}. {BoiPls.Key} [{BoiPls.Value.Entries.Count}]\n" +
                            $"by {User.Username}#{User.Discriminator}\n\n";
                        PList.Add(BoiPls.Key, BoiPls.Value);
                        k++;
                        ii++;
                        if (ii == 5)
                        {
                            ii = 0;
                            emb2.WithDescription(Pls);
                            Pls = "";
                            pages.Add(new Page
                            {
                                Embed = emb2.Build()
                            });
                        }
                    }
                }
                if (ii != 5 && Pls.Length != 0)
                {
                    emb2.WithDescription(Pls);
                    pages.Add(new Page
                    {
                        Embed = emb2.Build()
                    });
                }
                inter.SendPaginatedMessage(ctx.Channel, ctx.User, pages, TimeSpan.FromMinutes(2.5), TimeoutBehaviour.DeleteMessage).Wait(0);
                var pl = await inter.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id
                && (Bot.Members[ctx.Member.Id].Playlists.Any(y => y.Key == x.Content
                || PList.ToList()[Convert.ToInt32(x.Content) - 1].Value != null)), TimeSpan.FromSeconds(30));
                if (Bot.Members[ctx.Member.Id].Playlists.Any(x => x.Key == pl.Message.Content))
                {
                    PlName = PList.ToList().First(x => x.Key == pl.Message.Content).Key;
                    selectedList = PList.ToList().First(x => x.Key == pl.Message.Content).Value;
                    await pl.Message.DeleteAsync();
                }
                else if (PList.ToList()[Convert.ToInt32(pl.Message.Content) - 1].Value != null)
                {
                    PlName = PList.ToList()[Convert.ToInt32(pl.Message.Content) - 1].Key;
                    selectedList = PList.ToList()[Convert.ToInt32(pl.Message.Content) - 1].Value;
                    await pl.Message.DeleteAsync();
                }
                else
                {
                    await pl.Message.DeleteAsync();
                    return;
                }
            }
            else
            {
                selectedList = Bot.Members[ctx.Member.Id].Playlists[list];
            }
            bool i = true;
            await ctx.RespondAsync("Depending on playlist size this can take a while, there will be another message when its done! (tho the first song will already be played!)");
            int j = selectedList.Entries.Count;
            string failedSongs = "These songs coulnt be added since they were either regionblocked or had broken Links:\n\n";
            foreach (var PlE in selectedList.Entries)
            {
                var Get = await Utilities.GetTrack(ctx, PlE.URL);
                if (Get == null)
                {
                    failedSongs += $"{PlE.Title}\n" +
                        $"by {PlE.Author} [Link]({PlE.URL})";
                    j--;
                    continue;
                }
                Bot.Guilds[ctx.Guild.Id].Queue.Add(new QueueBase
                {
                    Requester = ctx.Member,
                    RequestTime = DateTime.Now,
                    Track = Get[0]
                });
                if (i)
                {
                    if (!Bot.Guilds[ctx.Guild.Id].Playing)
                    {
                        var chn = ctx.Member.VoiceState?.Channel;
                        if (chn == null) return;
                        if (Bot.Guilds[ctx.Guild.Id].GuildConnection == null || !Bot.Guilds[ctx.Guild.Id].GuildConnection.IsConnected)
                        {
                            if (Bot.Guilds[ctx.Guild.Id].GuildConnection == null)
                            {
                                Bot.Guilds[ctx.Guild.Id].GuildConnection = await Bot.LLEU.ConnectAsync(chn);
                                Bot.Guilds[ctx.Guild.Id].GuildConnection.PlaybackFinished += LavalinkEvents.TrackEnd;
                            }
                            else
                            {
                                Bot.Guilds[ctx.Guild.Id].GuildConnection = await Bot.LLEU.ConnectAsync(chn);
                            }
                        }
                        Bot.Guilds[ctx.Guild.Id].Playing = true;
                        int nextSong = 0;
                        Random rnd = new Random();
                        if (Bot.Guilds[ctx.Guild.Id].Shuffle) nextSong = rnd.Next(0, Bot.Guilds[ctx.Guild.Id].Queue.Count);
                        if (Bot.Guilds[ctx.Guild.Id].RepeatAll)
                        {
                            Bot.Guilds[ctx.Guild.Id].RepeatAllPosition++; nextSong = Bot.Guilds[ctx.Guild.Id].RepeatAllPosition;
                            if (Bot.Guilds[ctx.Guild.Id].RepeatAllPosition == Bot.Guilds[ctx.Guild.Id].Queue.Count) { Bot.Guilds[ctx.Guild.Id].RepeatAllPosition = 0; nextSong = 0; }
                        }
                        Bot.Guilds[ctx.Guild.Id].CurrentSong = Bot.Guilds[ctx.Guild.Id].Queue[nextSong];
                        Bot.Guilds[ctx.Guild.Id].GuildConnection.Play(Bot.Guilds[ctx.Guild.Id].Queue[nextSong].Track);
                        string time = "";
                        if (Bot.Guilds[ctx.Guild.Id].CurrentSong.Track.Length.Hours < 1) time = Bot.Guilds[ctx.Guild.Id].CurrentSong.Track.Length.ToString(@"mm\:ss");
                        else time = Bot.Guilds[ctx.Guild.Id].CurrentSong.Track.Length.ToString(@"hh\:mm\:ss");
                        await ctx.RespondAsync($"Playing: **{Bot.Guilds[ctx.Guild.Id].CurrentSong.Track.Title}** by **{Bot.Guilds[ctx.Guild.Id].CurrentSong.Track.Author}** [{time}]\n" +
                    $"Requested by {ctx.Member.Username}");
                    }
                    i = false;
                }
            }
            await ctx.RespondAsync($"Added {j} to queue");
            if (j != selectedList.Entries.Count)
            {
                await ctx.RespondAsync(failedSongs);
            }
        }

        [Command("show")]
        public async Task PLEList(CommandContext ctx, [RemainingText]string list = null)
        {
            if (!Bot.Members[ctx.Member.Id].Playlists.Any())
            {
                return;
            }
            var inter = ctx.Client.GetInteractivity();
            Playlists selectedList = new Playlists();
            string PlName = "";
            if (list == null)
            {
                string Pls = "";
                int k = 1;
                var emb2 = new DiscordEmbedBuilder();
                emb2.WithColor(new DiscordColor("6785A9"));
                emb2.WithTitle($"Your Playlists");
                List<Page> pages = new List<Page>();
                Dictionary<string, Playlists> PList = new Dictionary<string, Playlists>();
                foreach (var pls in Bot.Members[ctx.Member.Id].Playlists)
                {
                    Pls += k + ". " + pls.Key + $" ({pls.Value.Entries.Count} Songs)\n";
                    k++;
                    PList.Add(pls.Key, pls.Value);
                }
                emb2.WithDescription(Pls);
                pages.Add(new Page
                {
                    Embed = emb2.Build()
                });
                emb2.WithTitle($"Public Playlists");
                int i = 0;
                Pls = "";
                foreach (var PPls in Bot.Members.Where(x => x.Value.Playlists.Any(y => y.Value.Public) && x.Key != ctx.Member.Id))
                {
                    var User = await ctx.Client.GetUserAsync(PPls.Key);
                    foreach (var BoiPls in PPls.Value.Playlists.Where(x => x.Value.Public))
                    {
                        Pls += $"{k}. {BoiPls.Key} [{BoiPls.Value.Entries.Count}]\n" +
                            $"by {User.Username}#{User.Discriminator}\n\n";
                        PList.Add(BoiPls.Key, BoiPls.Value);
                        k++;
                        i++;
                        if (i == 5)
                        {
                            i = 0;
                            emb2.WithDescription(Pls);
                            Pls = "";
                            pages.Add(new Page
                            {
                                Embed = emb2.Build()
                            });
                        }
                    }
                }
                if (i != 5 && Pls.Length != 0)
                {
                    emb2.WithDescription(Pls);
                    pages.Add(new Page
                    {
                        Embed = emb2.Build()
                    });
                }
                inter.SendPaginatedMessage(ctx.Channel, ctx.User, pages, TimeSpan.FromMinutes(2.5), TimeoutBehaviour.DeleteMessage).Wait(0);
                var pl = await inter.WaitForMessageAsync(x => x.Author.Id == ctx.Member.Id
                && (Bot.Members[ctx.Member.Id].Playlists.Any(y => y.Key == x.Content
                || PList.ToList()[Convert.ToInt32(x.Content) - 1].Value != null)), TimeSpan.FromSeconds(30));
                if (Bot.Members[ctx.Member.Id].Playlists.Any(x => x.Key == pl.Message.Content))
                {
                    PlName = PList.ToList().First(x => x.Key == pl.Message.Content).Key;
                    selectedList = PList.ToList().First(x => x.Key == pl.Message.Content).Value;
                    await pl.Message.DeleteAsync();
                }
                else if (PList.ToList()[Convert.ToInt32(pl.Message.Content) - 1].Value != null)
                {
                    PlName = PList.ToList()[Convert.ToInt32(pl.Message.Content) - 1].Key;
                    selectedList = PList.ToList()[Convert.ToInt32(pl.Message.Content) - 1].Value;
                    await pl.Message.DeleteAsync();
                }
                else
                {
                    await pl.Message.DeleteAsync();
                    return;
                }
            }
            else
            {
                selectedList = Bot.Members[ctx.Member.Id].Playlists.First(x => x.Key == list).Value;
                PlName = Bot.Members[ctx.Member.Id].Playlists.First(x => x.Key == list).Key;
            }
            var emb = new DiscordEmbedBuilder();
            emb.WithColor(new DiscordColor("6785A9"));
            emb.WithTitle($"songs in {PlName}");
            int songsPerPage = 0;
            int currentPage = 1;
            int songAmount = 0;
            int totalP = selectedList.Entries.Count / 5;
            if ((selectedList.Entries.Count % 5) != 0) totalP++;
            List<Page> Pages = new List<Page>();
            string stuff = "";
            foreach (var Track in selectedList.Entries)
            {
                stuff += $"**{songAmount + 1}.{Track.Title}** by {Track.Author}\n" +
                    $"[Link]({Track.URL})\n˘˘˘˘˘\n";
                songsPerPage++;
                songAmount++;
                if (songsPerPage == 5)
                {
                    songsPerPage = 0;
                    emb.WithDescription(stuff);
                    emb.WithFooter($"Page {currentPage}/{totalP}");
                    Pages.Add(new Page
                    {
                        Embed = emb.Build()
                    });
                    stuff = "";
                    currentPage++;
                }
                if (songAmount == selectedList.Entries.Count)
                {
                    emb.WithDescription(stuff);
                    emb.WithFooter($"Page {currentPage++}/{totalP}");
                    Pages.Add(new Page
                    {
                        Embed = emb.Build()
                    });
                }
            }
            if (currentPage == 1)
            {
                emb.WithDescription(stuff);
                await ctx.RespondAsync(embed: emb.Build());
                return;
            }
            else if (currentPage == 2 && songsPerPage == 0)
            {
                await ctx.RespondAsync(embed: Pages.First().Embed);
                return;
            }
            foreach (var eP in Pages.Where(x => x.Embed.Description.Length == 0))
            {
                Pages.Remove(eP);
            }
            await inter.SendPaginatedMessage(ctx.Channel, ctx.User, Pages, TimeSpan.FromMinutes(5), TimeoutBehaviour.DeleteReactions);
        }
    }
}
