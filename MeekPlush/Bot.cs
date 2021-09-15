using DisCatSharp;
using DisCatSharp.CommandsNext;
using DisCatSharp.Entities;
using DisCatSharp.EventArgs;
using DisCatSharp.Interactivity;
using DisCatSharp.Interactivity.Enums;
using DisCatSharp.Interactivity.Extensions;
using DisCatSharp.Lavalink;
using DisCatSharp.Net;

using Microsoft.Extensions.Logging;

using MySql.Data.MySqlClient;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using static MeekPlush.Commands.XedddSpec;

namespace MeekPlush
{
    class Bot : IDisposable
    {
        static DiscordShardedClient bot { get; set; }
        static CancellationTokenSource _cts { get; set; }
        public static Dictionary<ulong, GuildList> Guilds = new Dictionary<ulong, GuildList>();
        public static Dictionary<ulong, MemberList> Members = new Dictionary<ulong, MemberList>();
        public static LavalinkNodeConnection LLEU { get; set; }

        public Bot()
        {
            Console.WriteLine("first");
            _cts = new CancellationTokenSource();
            bot = new DiscordShardedClient(new DiscordConfiguration
            {
                TokenType = TokenType.Bot,
                MinimumLogLevel = LogLevel.Debug
            });

            bot.MessageCreated += Despacito;
            bot.MessageCreated += UWUReact;
            bot.MessageCreated += Bot_Dump;
            bot.GuildMemberRemoved += Bot_XedddBoiLeave;
            bot.GuildDownloadCompleted += async (s, e) => {
                setGame().Wait(1000);
                InitNew(s, e).Wait(1000);
                Console.WriteLine("e");
                await Task.CompletedTask;
            };
            bot.VoiceStateUpdated += async (s, e) => {
                if (Guilds[e.Guild.Id].GuildConnection?.IsConnected  == false || Guilds[e.Guild.Id].GuildConnection == null) { return; }
                if (e?.Before?.Channel?.Id == Guilds[e.Guild.Id].GuildConnection?.Channel?.Id
                || e?.After?.Channel?.Id == Guilds[e.Guild.Id].GuildConnection?.Channel?.Id
                || e?.Channel?.Id == Guilds[e.Guild.Id].GuildConnection?.Channel?.Id)
                {
                    if (Guilds[e.Guild.Id].GuildConnection?.Channel?.Users.Where(x => !x.IsBot).Count() == 0)
                    {
                        Guilds[e.Guild.Id].Alone = true;
                        await AutoDisconnect(e.Guild.Id);
                        await Guilds[e.Guild.Id].GuildConnection.PauseAsync();
                        Guilds[e.Guild.Id].Paused = true;
                        await e.Guild.GetChannel(Guilds[e.Guild.Id].UsedChannel).SendMessageAsync($"Playback was paused since everybody left the Voice Channel, to unpause use ``{Guilds[e.Guild.Id].Prefix}resume`` otherwise I'll also disconnect in ~5min");
                    }
                    else
                    {
                        Guilds[e.Guild.Id].Alone = false;
                    }
                }
            };
            bot.ClientErrored += (s, e) =>
            {
                Console.WriteLine(e.Exception.Message);
                Console.WriteLine(e.Exception.StackTrace);
                return Task.CompletedTask;
            };
        }

        public async Task setGame()
        {
            await Task.CompletedTask;
            while (true)
            {
                DiscordActivity test = new DiscordActivity
                {
                    Name = "New Commands! check m!help",
                    ActivityType = ActivityType.Playing
                };
                await bot.UpdateStatusAsync(activity: test, userStatus: UserStatus.Online);
                await Task.Delay(TimeSpan.FromMinutes(30));
            }
        }

        public async Task<int> PrefixRes(DiscordMessage msg)
        {
            if (msg.Author.IsBot)
            {
                return -1;
            }
            if (!Members.Any(x => x.Key == msg.Author.Id))
            {
                await MissingMember(msg);
            }
            if (!Guilds.Any(x => x.Key == msg.Channel.Guild?.Id) && msg.Channel.Guild != null)
            {
                await MissingGuild(msg);
            }
            if (msg.Channel.Guild == null)
            {
                if (Members[msg.Author.Id].Prefix != null)
                {
                    if (msg.Content.ToLower().StartsWith(Members[msg.Author.Id].Prefix))
                    {
                        return msg.GetStringPrefixLength(Members[msg.Author.Id].Prefix);
                    }
                }
                if (msg.Content.ToLower().StartsWith("m!"))
                {
                    return msg.GetStringPrefixLength("m!");
                }

            }
            else
            {
                if (Members[msg.Author.Id].Prefix != null)
                {
                    if (msg.Content.ToLower().StartsWith(Members[msg.Author.Id].Prefix))
                    {
                        return msg.GetStringPrefixLength(Members[msg.Author.Id].Prefix);
                    }
                }
                if (msg.Content.ToLower().StartsWith(Guilds[msg.Channel.Guild.Id].Prefix))
                {
                    return msg.GetStringPrefixLength(Guilds[msg.Channel.Guild.Id].Prefix);
                }
            }
            return -1;
        }

        public async Task AutoDisconnect(ulong GuildId)
        {
            var DCT = DateTime.Now;
            var DCT2 = DateTime.Now;
            while (Guilds[GuildId].GuildConnection?.Channel?.Users.Where(x => !x.IsBot).Count() == 0 || Guilds[GuildId].Queue.Count < 1)
            {
                if (DCT.Subtract(DCT2).Minutes == 5)
                {
                    await Guilds[GuildId].GuildConnection.DisconnectAsync();
                    Guilds[GuildId].Repeat = false;
                    Guilds[GuildId].RepeatAll = false;
                    Guilds[GuildId].Shuffle = false;
                    Guilds[GuildId].Queue.Clear();
                    Guilds[GuildId].Paused = false;
                    Guilds[GuildId].Playing = false;
                    Guilds[GuildId].CurrentSong = new QueueBase();
                    Guilds[GuildId].GuildConnection.PlaybackFinished -= Events.MusicCommands.LavalinkEvents.TrackEnd;
                    Guilds[GuildId].GuildConnection = null;
                }
                else
                {
                    DCT = DateTime.Now;
                }
                await Task.Delay(10000);
            }
        }

        public async Task Despacito(DiscordClient c, MessageCreateEventArgs e)
        {
            if (e.Message.Content.ToLower().StartsWith("this is so sad alexa play despacito")
                || e.Message.Content.ToLower().StartsWith("this is so sad, alexa play despacito")
                || e.Message.Content.ToLower().StartsWith("this is so sad,alexa play despacito"))
            {
                try
                {
                    var whc = await e.Channel.CreateWebhookAsync("Alexa");
                    await whc.ExecuteAsync(new DiscordWebhookBuilder().WithContent("https://www.youtube.com/watch?v=kJQP7kiw5Fk").WithAvatarUrl("https://images-eu.ssl-images-amazon.com/images/I/41iz5Tw82IL._SY300_QL70_.jpg"));
                    await whc.DeleteAsync();
                }
                catch { }
            }
        }

        public async Task UWUReact(DiscordClient c, MessageCreateEventArgs e)
        {
            if (e.Message.Content.ToLower().StartsWith("uwu"))
            {
                await e.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("🇴"));
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                await e.Message.CreateReactionAsync(DiscordEmoji.FromUnicode("🇼"));
                await Task.Delay(TimeSpan.FromMilliseconds(500));
                await e.Message.CreateReactionAsync(DiscordEmoji.FromGuildEmote(c, 455504120825249802));
            }
        }

        public async Task RunBot()
        {
            await bot.StartAsync();
            var LL = await bot.UseLavalinkAsync();
            LLEU = await LL.First().Value.ConnectAsync(new LavalinkConfiguration
            {
                SocketEndpoint = new ConnectionEndpoint { Hostname = "localhost", Port = 8089 },
                RestEndpoint = new ConnectionEndpoint { Hostname = "localhost", Port = 2335 },
                Password = "youshallnotpass"
            });
            while (!_cts.IsCancellationRequested)
            {
                await Task.Delay(25);
            }
            foreach (var shard in bot.ShardClients)
            {
                await shard.Value.DisconnectAsync();
            }
        }

        public async Task InitNew(DiscordClient c, GuildDownloadCompletedEventArgs e)
        {
            await Task.Delay(1500);
            await Task.CompletedTask;
            try
            {
                var DBCon = new MySqlConnection();
                await DBCon.OpenAsync();
                foreach (var guild in e.Guilds.ToList())
                {
                    //Guilds
                    Console.WriteLine(guild.Value.Name);
                    try
                    {
                        MySqlCommand addGuild = new MySqlCommand
                        {
                            Connection = DBCon,
                            CommandText = $"INSERT INTO `guilds` (`GuildID`, `GuildName`, `Prefix`) VALUES (?, ?, 'm!') ON DUPLICATE KEY UPDATE GuildName=GuildName"
                        };
                        addGuild.Parameters.Add("GuildID", MySqlDbType.Int64).Value = guild.Value.Id;
                        addGuild.Parameters.Add("GuildName", MySqlDbType.VarChar).Value = guild.Value.Name;
                        await addGuild.ExecuteNonQueryAsync();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
                await DBCon.CloseAsync();
                foreach (var guild in e.Guilds.ToList())
                {
                    await DBCon.OpenAsync();
                    try
                    {
                        MySqlCommand lookGuild = new MySqlCommand
                        {
                            Connection = DBCon,
                            CommandText = $"SELECT * FROM `guilds` WHERE `GuildID` = {guild.Value.Id.ToString()}"
                        };
                        var reader = await lookGuild.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            Guilds.Add(guild.Value.Id, new GuildList
                            {
                                GuildName = guild.Value.Name,
                                Prefix = reader["Prefix"].ToString(),
                                Repeat = false,
                                RepeatAll = false,
                                Shuffle = false,
                                Playing = false,
                                RepeatAllPosition = 0,
                                Stop = false,
                                Paused = false,
                                Alone = false,
                                UsedChannel = 0,
                                Queue = new List<QueueBase>(),
                                CurrentSong = new QueueBase()
                            });
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex);
                        Console.WriteLine(ex.StackTrace);
                    }
                    await DBCon.CloseAsync();
                }

                //Get Users with PLs
                await DBCon.OpenAsync();
                try
                {
                    MySqlCommand lookGuild = new MySqlCommand
                    {
                        Connection = DBCon,
                        CommandText = $"SELECT DISTINCT UserID FROM `playlists` ;"
                    };
                    var reader = await lookGuild.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        await UserWithPL(Convert.ToUInt64(reader["UserID"]));
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                }
                await DBCon.CloseAsync();
                try
                {
                    Console.WriteLine("INT DB Done");
                    await bot.UseInteractivityAsync(new InteractivityConfiguration
                    {
                        PaginationDeletion = PaginationDeletion.DeleteEmojis
                    });
                    var CNextClients = await bot.UseCommandsNextAsync(new CommandsNextConfiguration
                    {
                        EnableDefaultHelp = false,
                        //StringPrefixes = new[] { "b!" },
                        PrefixResolver = PrefixRes
                    });
                    foreach (var cmd in CNextClients)
                    {
                        cmd.Value.RegisterCommands<Commands.Other>();
                        cmd.Value.RegisterCommands<Commands.Util>();
                        cmd.Value.RegisterCommands<Commands.XedddSpec>();
                        cmd.Value.RegisterCommands<Commands.YouTube>();
                        cmd.Value.RegisterCommands<Commands.YTDLC>();
                        cmd.Value.RegisterCommands<Commands.VUTDB>();
                        cmd.Value.RegisterCommands<Commands.RanImg.Other>();
                        cmd.Value.RegisterCommands<Commands.RanImg.RanList>();
                        cmd.Value.RegisterCommands<MusicCommands.MyList>();
                        cmd.Value.RegisterCommands<MusicCommands.Base>();
                        cmd.Value.RegisterCommands<MusicCommands.Music>();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
                Console.WriteLine("Commands Registered");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public async Task MissingGuild(DiscordMessage msg)
        {
            var DBCon = new MySqlConnection();
            await DBCon.OpenAsync();
            try
            {
                MySqlCommand addGuild = new MySqlCommand
                {
                    Connection = DBCon,
                    CommandText = $"INSERT INTO `guilds` (`GuildID`, `GuildName`, `Prefix`) VALUES (?, ?, 'm!') ON DUPLICATE KEY UPDATE GuildName=GuildName"
                };
                addGuild.Parameters.Add("GuildID", MySqlDbType.Int64).Value = msg.Channel.Guild.Id;
                addGuild.Parameters.Add("GuildName", MySqlDbType.VarChar).Value = msg.Channel.Guild.Name;
                await addGuild.ExecuteNonQueryAsync();
                try
                {
                    MySqlCommand lookGuild = new MySqlCommand
                    {
                        Connection = DBCon,
                        CommandText = $"SELECT * FROM `guilds` WHERE `GuildID` = {msg.Channel.Guild.Id.ToString()}"
                    };
                    var reader = await lookGuild.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        Guilds.Add(msg.Channel.Guild.Id, new GuildList
                        {
                            GuildName = msg.Channel.Guild.Name,
                            Prefix = reader["Prefix"].ToString(),
                            Repeat = false,
                            RepeatAll = false,
                            Shuffle = false,
                            Playing = false,
                            RepeatAllPosition = 0,
                            Stop = false,
                            Paused = false,
                            Alone = false,
                            UsedChannel = 0,
                            Queue = new List<QueueBase>(),
                            CurrentSong = new QueueBase()
                        });
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            await DBCon.CloseAsync();
            Console.WriteLine("Guild Insert Done");
        }

        public async Task MissingMember(DiscordMessage msg)
        {
            var e = msg.Author;
            if (!Members.Any(x => x.Key == e.Id) && !e.IsBot)
            {
                var DBCon = new MySqlConnection();
                await DBCon.OpenAsync();
                try
                {
                    MySqlCommand addMember = new MySqlCommand
                    {
                        Connection = DBCon,
                        CommandText = $"INSERT INTO `users` (`ID`, `Prefix`) VALUES (?, NULL) ON DUPLICATE KEY UPDATE ID=ID"
                    };
                    addMember.Parameters.Add("ID", MySqlDbType.Int64).Value = e.Id;
                    await addMember.ExecuteNonQueryAsync();
                    try
                    {
                        MySqlCommand lookMember = new MySqlCommand
                        {
                            Connection = DBCon,
                            CommandText = $"SELECT * FROM `users` WHERE `ID` = {e.Id.ToString()}"
                        };
                        var reader = await lookMember.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            MemberList test = new MemberList { Prefix = null };
                            if (reader["Prefix"].ToString().ToLower().Length != 0) test.Prefix = reader["Prefix"].ToString();
                            Members.Add(e.Id, new MemberList
                            {
                                Prefix = test.Prefix,
                                Playlists = new Dictionary<string, Playlists>()
                            });
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex);
                        Console.WriteLine(ex.StackTrace);
                    }
                    await DBCon.CloseAsync();
                    await DBCon.OpenAsync();
                    try
                    {
                        MySqlCommand lookPL = new MySqlCommand
                        {
                            Connection = DBCon,
                            CommandText = $"SELECT * FROM `playlists` WHERE `UserID` = {e.Id.ToString()}"
                        };
                        var reader = await lookPL.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            Members[e.Id].Playlists.Add(reader["PlaylistName"].ToString(), new Playlists());
                            var PLCon = new MySqlConnection();
                            await PLCon.OpenAsync();
                            MySqlCommand lookPLE = new MySqlCommand
                            {
                                Connection = PLCon,
                                CommandText = $"SELECT * FROM `playlistEntries` WHERE `UserID` = {e.Id.ToString()} AND `PlaylistName` = @list ORDER BY Position ASC"
                            };
                            lookPLE.Parameters.AddWithValue("@list", Convert.ToString(reader["PlaylistName"]));
                            var readere = await lookPLE.ExecuteReaderAsync();
                            while (await readere.ReadAsync())
                            {
                                Members[e.Id].Playlists[reader["PlaylistName"].ToString()].Entries.Add(new PlaylistEntrys
                                {
                                    Author = readere["Author"].ToString(),
                                    Title = readere["Title"].ToString(),
                                    URL = readere["URL"].ToString()
                                });
                                Members[e.Id].Playlists[reader["PlaylistName"].ToString()].Public = Convert.ToBoolean(reader["Public"]);
                            } 
                            readere.Close();
                            await PLCon.CloseAsync();
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                }
                await DBCon.CloseAsync();
            }
        }

        public async Task UserWithPL(ulong ID)
        {
            if (!Members.Any(x => x.Key == ID))
            {
                var DBCon = new MySqlConnection();
                await DBCon.OpenAsync();
                try
                {
                    MySqlCommand addMember = new MySqlCommand
                    {
                        Connection = DBCon,
                        CommandText = $"INSERT INTO `users` (`ID`, `Prefix`) VALUES (?, NULL) ON DUPLICATE KEY UPDATE ID=ID"
                    };
                    addMember.Parameters.Add("ID", MySqlDbType.Int64).Value = ID;
                    await addMember.ExecuteNonQueryAsync();
                    try
                    {
                        MySqlCommand lookMember = new MySqlCommand
                        {
                            Connection = DBCon,
                            CommandText = $"SELECT * FROM `users` WHERE `ID` = {ID.ToString()}"
                        };
                        var reader = await lookMember.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            MemberList test = new MemberList { Prefix = null };
                            if (reader["Prefix"].ToString().ToLower().Length != 0) test.Prefix = reader["Prefix"].ToString();
                            Members.Add(ID, new MemberList
                            {
                                Prefix = test.Prefix,
                                Playlists = new Dictionary<string, Playlists>()
                            });
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex);
                        Console.WriteLine(ex.StackTrace);
                    }
                    await DBCon.CloseAsync();
                    await DBCon.OpenAsync();
                    try
                    {
                        MySqlCommand lookPL = new MySqlCommand
                        {
                            Connection = DBCon,
                            CommandText = $"SELECT * FROM `playlists` WHERE `UserID` = {ID.ToString()}"
                        };
                        var reader = await lookPL.ExecuteReaderAsync();
                        while (await reader.ReadAsync())
                        {
                            Members[ID].Playlists.Add(reader["PlaylistName"].ToString(), new Playlists());
                            var PLCon = new MySqlConnection();
                            await PLCon.OpenAsync();
                            MySqlCommand lookPLE = new MySqlCommand
                            {
                                Connection = PLCon,
                                CommandText = $"SELECT * FROM `playlistEntries` WHERE `UserID` = {ID.ToString()} AND `PlaylistName` = @list ORDER BY Position ASC"
                            };
                            lookPLE.Parameters.AddWithValue("@list", Convert.ToString(reader["PlaylistName"]));
                            var readere = await lookPLE.ExecuteReaderAsync();
                            while (await readere.ReadAsync())
                            {
                                Members[ID].Playlists[reader["PlaylistName"].ToString()].Entries.Add(new PlaylistEntrys
                                {
                                    Author = readere["Author"].ToString(),
                                    Title = readere["Title"].ToString(),
                                    URL = readere["URL"].ToString()
                                });
                                Members[ID].Playlists[reader["PlaylistName"].ToString()].Public = Convert.ToBoolean(reader["Public"]);
                            }
                            readere.Close();
                            await PLCon.CloseAsync();
                        }
                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex);
                        Console.WriteLine(ex.StackTrace);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex);
                    Console.WriteLine(ex.StackTrace);
                }
                await DBCon.CloseAsync();
            }
        }

        public async Task Bot_XedddBoiLeave(DiscordClient c, GuildMemberRemoveEventArgs e)
        {
            if (e.Guild.Id == 373635826703400960)
            {
                string leaver = e.Member.DisplayName + " (" + e.Member.Nickname + ") " + "left uwu";
                var chn = await bot.ShardClients.First(x => x.Value.Guilds.Any(y => y.Value.Id == 373635826703400960)).Value.GetChannelAsync(391872124840574986);
                await chn.SendMessageAsync(leaver);
            }
        }

        public async Task Bot_Dump(DiscordClient c, MessageCreateEventArgs e)
        {
            try
            {
                if (!(e.Message.Channel.Type.ToString() == "Private"))
                {
                    StreamReader r = new StreamReader("XedddGroups.json");
                    string json = r.ReadToEnd();
                    var roles = JsonConvert.DeserializeObject<List<RootObject>>(json);
                    if (e.Guild.Id == 373635826703400960)
                    {
                        if (roles.Any(x => Convert.ToUInt64(x.ChannelID) == e.Channel.Id))
                        {
                            string proxy_urls = "";
                            string attlist = "";

                            if (e.Message.Attachments != null)
                            {
                                foreach (var files in e.Message.Attachments)
                                {
                                    proxy_urls += "\n  " + files.ProxyUrl;
                                }
                            }
                            if (!(proxy_urls == "")) attlist = proxy_urls;
                            if (e.Message.Content.Contains("http"))
                            {
                                attlist += e.Message.Content;
                            }
                            if (attlist != "")
                            {
                                var chn = await bot.ShardClients.First(x => x.Value.Guilds.Any(y => y.Value.Id == 373635826703400960)).Value.GetChannelAsync(466715815866269716);
                                await chn.SendMessageAsync(attlist);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

            }
        }

        public void Dispose()
        {
            bot = null;
        }
    }
}
