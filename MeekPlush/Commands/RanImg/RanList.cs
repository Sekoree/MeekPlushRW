using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MeekPlush.Commands.RanImg
{
    [Group("images")]
    class RanList : BaseCommandModule
    {
        [GroupCommand]
        public async Task ImgGroup(CommandContext ctx)
        {
            var emb = new DiscordEmbedBuilder();
            emb.WithTitle("MeekPlush Help");
            string prefix = "m!";
            if (Bot.Guilds[ctx.Guild.Id].Prefix != "m!" && Bot.Members[ctx.Member.Id].Prefix != "m!") prefix = Bot.Members[ctx.Member.Id].Prefix;
            else if (Bot.Guilds[ctx.Guild.Id].Prefix != "m!" && Bot.Members[ctx.Member.Id].Prefix == "m!") prefix = Bot.Guilds[ctx.Guild.Id].Prefix;
            else if (Bot.Guilds[ctx.Guild.Id].Prefix == "m!" && Bot.Members[ctx.Member.Id].Prefix != "m!") prefix = Bot.Members[ctx.Member.Id].Prefix;
            emb.WithThumbnailUrl(ctx.Client.CurrentUser.AvatarUrl);
            emb.WithDescription($"**Image Commands**");
            emb.AddField("__Animals__", $"**{prefix}cat** || Shows you a random Cat Picture\n" +
                $"**{prefix}dog** || Shows you a random Dog Picture", true);
            emb.AddField("__Vocaloid Pictures__", $"**{prefix}i diva** || Random Project Diva Image\n" +
                $"**{prefix}rin** || Random Kagamine Rin Image\n" +
                $"**{prefix}una** || Random Otomachi Una Image\n" +
                $"**{prefix}gumi** || Random GUMI Image\n" +
                $"**{prefix}luka** || Random Megurine Luka Image\n" +
                $"**{prefix}ia** || Random IA Image\n" +
                $"**{prefix}yukari** || Random Yuzuki Yukari Image\n" +
                $"**{prefix}teto** || Random Kasane Teto Image\n" +
                $"**{prefix}len** || Random Kagamine Len Image\n" +
                $"**{prefix}kaito** || Random Kaito Image\n" +
                $"**{prefix}meiko** || Random Meiko Image\n" +
                $"**{prefix}fukase** || Random Fukase Image\n" +
                $"**{prefix}miku** || Random Hatsune Miku Image\n" +
                $"**{prefix}miki** || Random SF-A2 Miku Image\n" +
                $"**{prefix}mayu** || Random Mayu Image\n" +
                $"**{prefix}aoki** || Random Aoki Lapis Image\n" +
                $"**{prefix}lily** || Random Lily Image", true);
            emb.AddField("__Other Images__", $"**{prefix}neko** || Random Catgirl Image\n" +
                $"**{prefix}kanna** || Random Kanna Image\n", true);
            emb.AddField("__NSFW Images__", $"**{prefix}i thigh** || Random Thigh Image\n" +
                $"**{prefix}nekopara** || Random Nekopara Image\n", true);
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
    }
}
