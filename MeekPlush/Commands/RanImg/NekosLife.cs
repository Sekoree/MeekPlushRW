using DisCatSharp.CommandsNext;
using DisCatSharp.CommandsNext.Attributes;
using DisCatSharp.Entities;

using Newtonsoft.Json;

using System.IO;
using System.Net;
using System.Threading.Tasks;

using static HeyRed.Mime.MimeTypesMap;

namespace MeekPlush.Commands.RanImg
{
    partial class Other : BaseCommandModule
    {
        [Command("neko"), Description("Show you a Random catgirl image")]
        public async Task NekoPic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("https://nekos.life/api/v2/img/neko");
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
                Color = new DiscordColor("#68D3D2"),
                Title = "Random Catgirl Pictures!",
                Description = $"[Full Source Image Link]({myresponse.url.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            response.Close();
            emim.WithAuthor(name: "via nekos.life", url: "https://nekos.life/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);
            DiscordMessageBuilder builder = new DiscordMessageBuilder();
            builder.WithFile($"image.{GetExtension(response2.ContentType)}", dataStream2);
            builder.WithEmbed(emim.Build());
            await ctx.RespondAsync(builder);
        }

        [Command("nl"), RequireNsfw, Description("get a random picture from nekos.life (category name required, look at point 11 of https://nekos.life/api/v2/endpoints) nneds NSFW Channel, due to this random pic database beim 2/3 porn lmao")]
        public async Task nekosLife(CommandContext ctx, string pick)
        {
            WebRequest request = WebRequest.Create("https://nekos.life/api/v2/img/" + pick);
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            //await ctx.RespondAsync(responseFromServer);
            var myresponse = JsonConvert.DeserializeObject<Other.ImgRet>(responseFromServer);
            //await ctx.RespondAsync(myresponse.url);

            var emim = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#6600cc"),
                Title = "Random Pictures!",
                Description = $"[Full Source Image Link]({myresponse.url.ToString()})",
                ImageUrl = $"https://api.meek.moe/im/?image={myresponse.url.ToString()}&resize=500"
            };
            response.Close();
            emim.WithAuthor(name: "via nekos.life", url: "https://nekos.life/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);

            await ctx.RespondAsync(embed: emim.Build());
        }
    }
}
