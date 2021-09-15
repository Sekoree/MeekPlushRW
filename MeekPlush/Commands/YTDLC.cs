using DisCatSharp.CommandsNext;
using DisCatSharp.CommandsNext.Attributes;

using NYoutubeDL;

using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MeekPlush.Commands
{
    [Group("download"), Aliases("dl")]
    class YTDLC : BaseCommandModule
    {
        [GroupCommand]
        public async Task DLGroup(CommandContext ctx)
        {
            await ctx.RespondAsync("dl stuff");
        }

        [Command("vdl"), RequireOwner]
        public async Task YTDL(CommandContext ctx, string link)
        {
            if (link.Contains("youtu") || link.Contains("nico"))
            {
                var msg = await ctx.RespondAsync("Download started!");
                System.Random random = new System.Random();
                const string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
                var chars = Enumerable.Range(0, 20)
                    .Select(x => pool[random.Next(0, pool.Length)]);
                string finalString = new string(chars.ToArray());
                var youtubeDl = new YoutubeDL();
                if (link.Contains("youtu"))
                {
                    youtubeDl.Options.FilesystemOptions.Output = $"/var/www/vhosts/srgg.de/why-is-this-a-me.me/ytdl/{finalString}.mp4";
                    youtubeDl.Options.VideoSelectionOptions.NoPlaylist = true;
                }

                if (link.Contains("nico"))
                {
                    youtubeDl.Options.FilesystemOptions.Output = $"/var/www/vhosts/srgg.de/why-is-this-a-me.me/nnddl/{finalString}.mp4";
                    youtubeDl.Options.FilesystemOptions.NoCacheDir = true;
                }
                youtubeDl.Options.PostProcessingOptions.ExtractAudio = true;
                youtubeDl.Options.VideoFormatOptions.Format = NYoutubeDL.Helpers.Enums.VideoFormat.best;
                youtubeDl.Options.PostProcessingOptions.ExtractAudio = true;
                youtubeDl.Options.PostProcessingOptions.AudioFormat = NYoutubeDL.Helpers.Enums.AudioFormat.mp3;
                youtubeDl.Options.PostProcessingOptions.AudioQuality = "320k";
                youtubeDl.Options.PostProcessingOptions.AddMetadata = true;
                youtubeDl.Options.PostProcessingOptions.EmbedThumbnail = true;
                youtubeDl.Options.GeneralOptions.Update = true;
                youtubeDl.VideoUrl = link;
                youtubeDl.YoutubeDlPath = "youtube-dl";
                youtubeDl.StandardErrorEvent += (sender, errorOutput) => ctx.RespondAsync($"{errorOutput}  (If its 403 blame NNDs Servers, hella slow sometimes and thus cancel the download uwu)");
                await youtubeDl.DownloadAsync();
                if (File.Exists($"/var/www/vhosts/srgg.de/why-is-this-a-me.me/ytdl/{finalString}.mp3"))
                {
                    await msg.ModifyAsync("https://why-is-this-a-me.me/ytdl/" + finalString + ".mp3 \n" +
                            "**This file will be deleted in about 30min!**");
                    await Task.Delay((60000 * 30));
                    File.Delete($"/var/www/vhosts/srgg.de/why-is-this-a-me.me/ytdl/{finalString}.mp3");
                }
                else if (File.Exists($"/var/www/vhosts/srgg.de/why-is-this-a-me.me/nnddl/{finalString}.mp3"))
                {
                    await msg.ModifyAsync("https://why-is-this-a-me.me/nnddl/" + finalString + ".mp3 \n" +
                            "**This file will be deleted in about 30min!**");
                    await Task.Delay((60000 * 30));
                    File.Delete($"/var/www/vhosts/srgg.de/why-is-this-a-me.me/nnddl/{finalString}.mp3");
                }
            }
            else
            {
                await ctx.RespondAsync("no YouTube or NND link detected, please try again");
            }
        }
    }
}
