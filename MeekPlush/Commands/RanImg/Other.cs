using DisCatSharp.CommandsNext;
using DisCatSharp.CommandsNext.Attributes;
using DisCatSharp.Entities;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using static HeyRed.Mime.MimeTypesMap;

namespace MeekPlush.Commands.RanImg
{
    partial class Other : BaseCommandModule
    {
        [Command("cat"), Description("Shows you a random cat picture")]
        public async Task CatPic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("http://thecatapi.com/api/images/get?format=src");
            WebResponse response = request.GetResponse();
            WebRequest request2 = WebRequest.Create($"https://api.meek.moe/im/?image={response.ResponseUri.ToString()}&resize=500");
            WebResponse response2 = await request2.GetResponseAsync();
            Stream dataStream2 = response2.GetResponseStream();
            var emim = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#68D3D2"),
                Title = "Random Cat Picture/Gif!",
                Description = $"[Full Source Image Link]({response.ResponseUri.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };

            response.Close();
            emim.WithAuthor(name: "via thecatapi.com", url: "https://thecatapi.com/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);
            DiscordMessageBuilder builder = new DiscordMessageBuilder();
            builder.WithFile($"image.{GetExtension(response2.ContentType)}", dataStream2);
            builder.WithEmbed(emim.Build());
            await ctx.RespondAsync(builder);
        }

        [Command("dog"), Description("Shows you a random dog picture")]
        public async Task DogPic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("https://api.thedogapi.com/v1/images/search");
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            var myresponse = JsonConvert.DeserializeObject<List<ImgRet>>(responseFromServer);
            response.Dispose();
            dataStream.Dispose();
            WebRequest request2 = WebRequest.Create($"https://api.meek.moe/im/?image={myresponse[0].url}&resize=500");
            WebResponse response2 = await request2.GetResponseAsync();
            Stream dataStream2 = response2.GetResponseStream();

            var emim = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#68D3D2"),
                Title = "Random Dog Picture/Gif!",
                Description = $"[Full Source Image Link]({myresponse[0].url})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            response.Close();
            emim.WithAuthor(name: "via thedogapi.com", url: "https://thedogapi.com/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);
            DiscordMessageBuilder builder = new DiscordMessageBuilder();
            builder.WithFile($"image.{GetExtension(response2.ContentType)}", dataStream2);
            builder.WithEmbed(emim.Build());
            await ctx.RespondAsync(builder);
        }
    }
}
