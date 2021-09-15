using DisCatSharp.CommandsNext;
using DisCatSharp.CommandsNext.Attributes;
using DisCatSharp.Entities;

using Newtonsoft.Json;

using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using static HeyRed.Mime.MimeTypesMap;

namespace MeekPlush.Commands.RanImg
{
    partial class Other : BaseCommandModule
    {
        [Command("nekopara"), Description("Shows you a random nekopara image or gif uwu"), RequireNsfw]
        public async Task NPPic(CommandContext ctx)
        {
            Random rnd = new Random();
            int yo = rnd.Next(0, 2);
            string url = "";
            if (yo == 0) url = "https://api.ohlookitsderpy.space/nekoparagif";
            else url = "https://api.ohlookitsderpy.space/nekoparastatic";
            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            var myresponse = JsonConvert.DeserializeObject<ImgRet>(responseFromServer);
            response.Dispose();
            dataStream.Dispose();
            WebRequest request2 = WebRequest.Create($"https://api.meek.moe/im/?image={myresponse.url.ToString()}&resize=500");
            WebResponse response2 = await request2.GetResponseAsync();
            Stream dataStream2 = response2.GetResponseStream();

            var emim = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#63E4E3"),
                Title = "uwu",
                Description = $"[Full Source Image Link]({myresponse.url.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            response.Close();
            emim.WithAuthor(name: "via api.ohlookitsderpy.space", url: "https://api.ohlookitsderpy.space/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);
            DiscordMessageBuilder builder = new DiscordMessageBuilder();
            builder.WithFile($"image.{GetExtension(response2.ContentType)}", dataStream2);
            builder.WithEmbed(emim.Build());
            await ctx.RespondAsync(builder);
        }
    }
}
