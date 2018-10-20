using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeekPlush.Commands
{
    class Other : BaseCommandModule
    {
        [Command("emotes")]
        public async Task ListEmotes(CommandContext ctx)
        {
            var inter = ctx.Client.GetInteractivity();
            var ems = ctx.Guild.Emojis;
            List<Page> emot = new List<Page>();
            int page = 1;
            var emb = new DiscordEmbedBuilder();
            foreach (DiscordEmoji e in ems)
            {
                emb.WithTitle($"``{e.GetDiscordName()}``");
                emb.WithImageUrl(e.Url);
                emb.WithFooter($"Page {page}/{ems.Count} || Requested by {ctx.Member.DisplayName ?? ctx.Member.Username}", ctx.User.AvatarUrl);
                emot.Add(new Page {
                    Embed = emb.Build()
                });
                page++;
            }
            await inter.SendPaginatedMessage(ctx.Channel, ctx.User, emot);
        }

        [Command("help")]
        public async Task Help(CommandContext ctx)
        {
            var emb = new DiscordEmbedBuilder();
            emb.WithTitle("MeekPlush Help");
            string prefix = "m!";
            if (Bot.Guilds[ctx.Guild.Id].Prefix != "m!" && Bot.Members[ctx.Member.Id].Prefix != "m!") prefix = Bot.Members[ctx.Member.Id].Prefix;
            else if (Bot.Guilds[ctx.Guild.Id].Prefix != "m!" && Bot.Members[ctx.Member.Id].Prefix == "m!") prefix = Bot.Guilds[ctx.Guild.Id].Prefix;
            else if (Bot.Guilds[ctx.Guild.Id].Prefix == "m!" && Bot.Members[ctx.Member.Id].Prefix != "m!") prefix = Bot.Members[ctx.Member.Id].Prefix;
            emb.WithThumbnailUrl(ctx.Client.CurrentUser.AvatarUrl);
            emb.WithDescription($"**All Commands and additional Help Commands!**\n\n" +
                $"**{prefix}info** || Info about the Bot and you!" +
                $"**{prefix}emotes** || Gives you a small 'catalog' of all custom emotes this server has!\n" +
                $"**{prefix}myprefix <newPrefix>** || Set a new Prefix for Yourself to use the bot with\n" +
                $"**{prefix}guildprefix <newPrefix>** || (Requires Admin Perms) Set the Prefix for this Guild\n" +
                $"**{prefix}vocadb <SearchTerm>** || Search a song on VocaDB\n" +
                $"**{prefix}music** || Display all Music commands\n" +
                $"**{prefix}images** || Display all Image commands\n" +
                $"**{prefix}mylist** || Display all Playlist commands\n" +
                $"**{prefix}youtube** || Display YouTube commands\n" +
                $"**{prefix}australian <Words?> || Translate something to Australian");
            emb.AddField("GuildInfo", $"Your prefix is: {Bot.Members[ctx.Member.Id].Prefix}\n" +
                $"This Guilds Prefix is: {Bot.Guilds[ctx.Guild.Id].Prefix}");
            emb.AddField("Info",$"Avatar by Chillow#1945 :heart: [Twitter](https://twitter.com/SaikoSamurai)\n" +
                $"Bot Github: soon™\n" +
                $"DBL: [Link](https://discordbots.org/bot/465675368775417856) every upvote helps and is appreciated uwu\n" +
                $"Support Server: [Invite](https://discord.gg/YPPA2Pu)\n" +
                $"Support me uwu on [Paypal](https://www.paypal.me/speyd3r) or [Patreon](https://www.patreon.com/speyd3r)");
            emb.WithColor(new DiscordColor("6785A9"));
            await ctx.RespondAsync(embed: emb.Build());
        }

        [Command("info")]
        public async Task Info(CommandContext ctx)
        {
            int t = 0;
            foreach (var pl in Bot.Members[ctx.Member.Id].Playlists)
            {
                t += pl.Value.Entries.Count;
            }
            var emb = new DiscordEmbedBuilder();
            emb.WithTitle("Info & Stats");
            emb.WithDescription($"Bot Author: Speyd3r#3939\n" +
                $"Avatar by Chillow#1945 :heart: [Twitter](https://twitter.com/SaikoSamurai)\n" +
                $"Bot Github: soon™\n" +
                $"DBL: [Link](https://discordbots.org/bot/465675368775417856) every upvote helps and is appreciated uwu\n" +
                $"Support Server: [Invite](https://discord.gg/YPPA2Pu)" +
                $"[Paypal](https://www.paypal.me/speyd3r) [Patreon](https://www.patreon.com/speyd3r)\n\n" +
                $"Your prefix is: {Bot.Members[ctx.Member.Id].Prefix}\n" +
                $"You have {Bot.Members[ctx.Member.Id].Playlists.Count} Playlists\n" +
                $"with a total of {t} Songs!\n\n" +
                $"Ping: {ctx.Client.Ping}\n" +
                $"Servers: {ctx.Client.Guilds.Count}");
            emb.WithColor(new DiscordColor("6785A9"));
            await ctx.RespondAsync(embed: emb.Build());
        }

        [Command("myprefix")]
        public async Task UserPrefix(CommandContext ctx, [RemainingText] string newPrefix = null)
        {
            Bot.Members[ctx.Member.Id].Prefix = newPrefix;
            try
            {
                var DBCon = new MySqlConnection();
                await DBCon.OpenAsync();
                MySqlCommand addGuild = new MySqlCommand
                {
                    Connection = DBCon,
                    CommandText = $"UPDATE `users` SET `Prefix` = @newName WHERE `users`.`ID` = {ctx.Member.Id.ToString()}",
                };
                addGuild.Parameters.AddWithValue("@newName", newPrefix);
                await addGuild.ExecuteNonQueryAsync();
                await DBCon.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            await ctx.RespondAsync("Your Personal prefix is now: " + newPrefix);
        }

        [Command("guildprefix"), RequireUserPermissions(DSharpPlus.Permissions.Administrator)]
        public async Task GuildPrefix(CommandContext ctx, [RemainingText] string newPrefix = "m!")
        {
            Bot.Guilds[ctx.Guild.Id].Prefix = newPrefix;
            try
            {
                var DBCon = new MySqlConnection();
                await DBCon.OpenAsync();
                MySqlCommand addGuild = new MySqlCommand
                {
                    Connection = DBCon,
                    CommandText = $"UPDATE `guilds` SET `Prefix` = @newName WHERE `guilds`.`GuildID` = {ctx.Guild.Id.ToString()}",
                };
                addGuild.Parameters.AddWithValue("@newName", newPrefix);
                await addGuild.ExecuteNonQueryAsync();
                await DBCon.CloseAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
                Console.WriteLine(ex.StackTrace);
            }
            await ctx.RespondAsync("The Guild prefix is now: " + newPrefix);
        }

        [Command("australian")]
        public async Task AUTrans(CommandContext ctx, [RemainingText]string text)
        {
            char[] X = @"¿/˙'\‾¡zʎxʍʌnʇsɹbdouɯlʞɾıɥƃɟǝpɔqɐ".ToCharArray();
            string V = @"?\.,/_!zyxwvutsrqponmlkjihgfedcba";
            await ctx.RespondAsync(new string((from char obj in text.ToCharArray()
                               select (V.IndexOf(obj) != -1) ? X[V.IndexOf(obj)] : obj).Reverse().ToArray()));
        }
    }
}
