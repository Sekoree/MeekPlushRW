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
        [Command("thigh"), Description("Shows you a random thigh image uwu"), RequireNsfw]
        public async Task ThighPic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("https://nekobot.xyz/api/image?type=thigh");
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            var myresponse = JsonConvert.DeserializeObject<NadekoRet>(responseFromServer);
            response.Dispose();
            dataStream.Dispose();
            WebRequest request2 = WebRequest.Create($"https://api.meek.moe/im/?image={myresponse.message.ToString()}&resize=500");
            WebResponse response2 = await request2.GetResponseAsync();
            Stream dataStream2 = response2.GetResponseStream();

            var emim = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#68D3D2"),
                Title = "uwu",
                Description = $"[Full Source Image Link]({myresponse.message.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            response.Close();
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);
            DiscordMessageBuilder builder = new DiscordMessageBuilder();
            builder.WithFile($"image.{GetExtension(response2.ContentType)}", dataStream2);
            builder.WithEmbed(emim.Build());
            await ctx.RespondAsync(builder);
        }

        [Command("kanna"), Description("Shows you a Kanna image")]
        public async Task KannaPic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("https://nekobot.xyz/api/image?type=kanna");
            WebResponse response = request.GetResponse();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            var myresponse = JsonConvert.DeserializeObject<NadekoRet>(responseFromServer);
            response.Dispose();
            dataStream.Dispose();
            WebRequest request2 = WebRequest.Create($"https://api.meek.moe/im/?image={myresponse.message.ToString()}&resize=500");
            WebResponse response2 = await request2.GetResponseAsync();
            Stream dataStream2 = response2.GetResponseStream();
            var emim = new DiscordEmbedBuilder
            {
                Color = new DiscordColor("#68D3D2"),
                Title = "uwu",
                Description = $"[Full Source Image Link]({myresponse.message.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            response.Close();
            emim.WithAuthor(name: "via nekobot.xyz", url: "https://nekobot.xyz/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);
            DiscordMessageBuilder builder = new DiscordMessageBuilder();
            builder.WithFile($"image.{GetExtension(response2.ContentType)}", dataStream2);
            builder.WithEmbed(emim.Build());
            await ctx.RespondAsync(builder);
        }

        public class NadekoRet
        {
            [JsonProperty("message")]
            public string message { get; set; }
        }
    }
}
