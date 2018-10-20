using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
//using OpenQA.Selenium;
//using OpenQA.Selenium.Firefox;

namespace MeekPlush.Commands
{
    [Group("youtube"), Aliases("yt")]
    class YouTube : BaseCommandModule
    {
        [GroupCommand]
        public async Task YTGroup(CommandContext ctx)
        {
            var emb = new DiscordEmbedBuilder();
            emb.WithTitle("MeekPlush Help");
            string prefix = "m!";
            if (Bot.Guilds[ctx.Guild.Id].Prefix != "m!" && Bot.Members[ctx.Member.Id].Prefix != "m!") prefix = Bot.Members[ctx.Member.Id].Prefix;
            else if (Bot.Guilds[ctx.Guild.Id].Prefix != "m!" && Bot.Members[ctx.Member.Id].Prefix == "m!") prefix = Bot.Guilds[ctx.Guild.Id].Prefix;
            else if (Bot.Guilds[ctx.Guild.Id].Prefix == "m!" && Bot.Members[ctx.Member.Id].Prefix != "m!") prefix = Bot.Members[ctx.Member.Id].Prefix;
            emb.WithThumbnailUrl(ctx.Client.CurrentUser.AvatarUrl);
            emb.WithDescription($"**Youtube Search Commands!**\n\n" +
                $"**{prefix}yt v <Searchterm>** || Searches a YouTube video\n" +
                $"**{prefix}yt c <Searchterm>** || Searches a YouTube video\n" +
                $"**{prefix}yt p <Searchterm>** || Searches a YouTube playlist");
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

        [Command("s"), Description("Search a YouTube Video!"), Aliases("v")]
        public async Task YtSearchV(CommandContext ctx, [Description("Searchterm"), RemainingText]string term)
        {
            try
            {
                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApplicationName = this.GetType().ToString()
                });

                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Q = term; 
                searchListRequest.MaxResults = 1;
                searchListRequest.Type = "video"; 
                var searchListResponse = await searchListRequest.ExecuteAsync();
                await ctx.RespondAsync("https://www.youtube.com/watch?v=" + searchListResponse.Items[0].Id.VideoId);
            }
            catch (AggregateException ex)
            {
                foreach (var e in ex.InnerExceptions)
                {
                    await ctx.RespondAsync("Error: " + e.Message);
                }
            }
        }

        [Command("c"), Description("Search a YouTube Channel!")]
        public async Task YtSearchC(CommandContext ctx, [Description("Searchterm"), RemainingText]string term)
        {
            if (ctx.Message.Content.EndsWith("!yt channel") || ctx.Message.Content.EndsWith("!yt channel ") || ctx.Message.Content.EndsWith("!yt c") || ctx.Message.Content.EndsWith("!yt c "))
            {
                await ctx.RespondAsync("plz enter search term e.g. ```!yt c channelname```");
            }
            else
            {
                try
                {
                    var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                    {
                        ApplicationName = this.GetType().ToString()
                    });

                    var searchListRequest = youtubeService.Search.List("snippet");
                    searchListRequest.Q = term;
                    searchListRequest.MaxResults = 1;
                    searchListRequest.Type = "channel";
                    var searchListResponse = await searchListRequest.ExecuteAsync();

                    await ctx.RespondAsync("https://www.youtube.com/channel/" + searchListResponse.Items[0].Id.ChannelId);
                }
                catch (AggregateException ex)
                {
                    foreach (var e in ex.InnerExceptions)
                    {
                        await ctx.RespondAsync("Error: " + e.Message);
                    }
                }
            }
        }

        [Command("p"), Description("Search a YouTube Playlist!")]
        public async Task YtSearchP(CommandContext ctx, [Description("Searchterm"), RemainingText]string term)
        {
            if (ctx.Message.Content.EndsWith("!yt list") || ctx.Message.Content.EndsWith("!yt list ") || ctx.Message.Content.EndsWith("!yt p") || ctx.Message.Content.EndsWith("!yt p ") || ctx.Message.Content.EndsWith("!yt l") || ctx.Message.Content.EndsWith("!yt l "))
            {
                await ctx.RespondAsync("plz enter search term e.g. ```!yt l playlistname```");
            }
            else
            {
                try
                {
                    var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                    {
                        ApplicationName = this.GetType().ToString()
                    });
                    var searchListRequest = youtubeService.Search.List("snippet");
                    searchListRequest.Q = term;
                    searchListRequest.MaxResults = 1;
                    searchListRequest.Type = "playlist";
                    var searchListResponse = await searchListRequest.ExecuteAsync();
                    await ctx.RespondAsync("https://www.youtube.com/playlist?list=" + searchListResponse.Items[0].Id.PlaylistId);
                }
                catch (AggregateException ex)
                {
                    foreach (var e in ex.InnerExceptions)
                    {
                        await ctx.RespondAsync("Error: " + e.Message);
                    }
                }
            }
        }

        /*
        [Command("nnd")]
        public async Task NND(CommandContext ctx, string listPick = null)
        {
            int offset = 0;
            if (listPick.ToLower() == "hourly") offset = 0;
            else if (listPick.ToLower() == "daily") offset = 20;
            else if (listPick.ToLower() == "weekly") offset = 40;
            else if (listPick.ToLower() == "monthly") offset = 60;
            var inter = ctx.Client.GetInteractivity();
            var init = await ctx.RespondAsync("this may take a while, be patient uwu");
            IWebDriver driver;
            var chromeDriverService = FirefoxDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true; //selenium shows a commandprompt, this hides it
            chromeDriverService.SuppressInitialDiagnosticInformation = true;
            FirefoxOptions options = new FirefoxOptions();
            options.AddArguments("--headless"); //to tell firefox to start in windowless mode
            driver = new FirefoxDriver(chromeDriverService, options); //starts up firefox it in the directory already (called gecko)
            driver.Navigate().GoToUrl("http://ex.nicovideo.jp/vocaloid/ranking"); //ig goes to the ranking pages
            string[] allranks = new string[80]; // there are always 80 songs on that page! this is for the names
            string[] allURLS = new string[80]; //it also gets the urls
            int yeet = 0; //to add stuff to the array
            DiscordColor meek = new DiscordColor("#68D3D2");
            foreach (var vids in driver.FindElements(By.ClassName("ttl"))) //lit ay? all items in the ranking have the classname ttl
            {
                allranks[yeet] = vids.Text; //it gest the inner text which is the Name
                allURLS[yeet] = driver.FindElement(By.LinkText(vids.Text)).GetAttribute("href"); //and the link from the "a" tag thats arond the Songname
                yeet++;
            }
            var empty = ctx.Client.GetGuildAsync(401419401011920907).Result.GetEmojiAsync(435447550196318218).Result; //i have an emoji on my server thats just blank, i use it for spaces in shit here
            List<Page> pgs = new List<Page>();
            DiscordEmbedBuilder bassc = new DiscordEmbedBuilder
            {
                Color = meek,
                Title = "NicoNicoDouga " + listPick + " Vocaloid Ranking",
                Description = "Top 20 " + listPick + " songs!\n" + empty,
                ThumbnailUrl = "https://japansaucedotnet.files.wordpress.com/2016/05/photo-22.gif"
            };
            //DiscordEmbedBuilder[] hourly = new DiscordEmbedBuilder[4];
            int adv = 0;
            int adv2 = 5;
            for (int j = 0; j < 4; j++)
            {
                if (j == 1)
                {
                    adv = 5;
                    adv2 = 10;
                }
                if (j == 2)
                {
                    adv = 10;
                    adv2 = 15;
                }
                if (j == 3)
                {
                    adv = 15;
                    adv2 = 20;
                }
                Console.WriteLine("whut");
                DiscordEmbedBuilder hourly = new DiscordEmbedBuilder
                {
                    Color = meek,
                    Title = "NicoNicoDouga " + listPick + " Vocaloid Ranking",
                    Description = "Top 20 " + listPick + " songs!\n" + empty,
                    ThumbnailUrl = "https://japansaucedotnet.files.wordpress.com/2016/05/photo-22.gif"
                };
                for (int i = (offset + adv); i < (offset + adv2); i++) //gets the first 5
                {
                    if (i < (offset + (adv2 - 1)))
                    {
                        hourly.AddField("#" + ((i - offset) + 1) + " " + allranks[i], allURLS[i]);
                    }
                    else
                    {
                        hourly.AddField("#" + ((i - offset) + 1) + " " + allranks[i], allURLS[i] + "\n" + empty);
                    }
                }
                hourly.AddField("Page", $"{(j + 1)}/4");
                pgs.Add(new Page { Embed = hourly.Build() });
            }
            driver.Quit();
            //hourly.AddField("Page", pages + "/4\nEnter Page number or type ``exit`` to exit");
            await init.DeleteAsync();
            await inter.SendPaginatedMessage(ctx.Channel, ctx.Message.Author, pgs, timeoutoverride: TimeSpan.FromMinutes(2));
        }*/
    }
}
