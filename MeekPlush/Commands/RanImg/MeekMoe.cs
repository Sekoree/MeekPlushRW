using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static HeyRed.Mime.MimeTypesMap;

namespace MeekPlush.Commands.RanImg
{
    partial class Other : BaseCommandModule
    {
        [Command("diva"), Description("Shows you a Project Diva image")]
        public async Task DivaPic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("https://api.meek.moe/diva");
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
                Title = "Random Project Diva Image!",
                Description = $"[Full Source Image Link]({myresponse.url.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            response.Close();
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);

            await ctx.RespondWithFileAsync(fileName: $"image.{GetExtension(response2.ContentType)}", fileData: dataStream2, embed: emim.Build());
        }

        [Command("rin"), Description("Shows you a Rin image")]
        public async Task KRinPic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("https://api.meek.moe/rin");
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
                Title = "Random Rin Image!",
                Description = $"[Full Source Image Link]({myresponse.url.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            if (myresponse.creator.Length != 0)
            {
                emim.AddField("Creator", myresponse.creator);
            }
            response.Close();
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);

            await ctx.RespondWithFileAsync(fileName: $"image.{GetExtension(response2.ContentType)}", fileData: dataStream2, embed: emim.Build());
        }

        [Command("una"), Description("Shows you a Una image")]
        public async Task OUnaPic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("https://api.meek.moe/una");
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
                Title = "Random Una Image!",
                Description = $"[Full Source Image Link]({myresponse.url.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            if (myresponse.creator.Length != 0)
            {
                emim.AddField("Creator", myresponse.creator);
            }
            response.Close();
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);
            await ctx.RespondWithFileAsync(fileName: $"image.{GetExtension(response2.ContentType)}", fileData: dataStream2, embed: emim.Build());
        }

        [Command("gumi"), Description("Shows you a Gumi image")]
        public async Task GumiPic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("https://api.meek.moe/gumi");
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
                Title = "Random Gumi Image!",
                Description = $"[Full Source Image Link]({myresponse.url.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            if (myresponse.creator.Length != 0)
            {
                emim.AddField("Creator", myresponse.creator);
            }
            response.Close();
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);
            await ctx.RespondWithFileAsync(fileName: $"image.{GetExtension(response2.ContentType)}", fileData: dataStream2, embed: emim.Build());
        }

        [Command("luka"), Description("Shows you a Luka image")]
        public async Task MLukaPic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("https://api.meek.moe/luka");
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
                Title = "Random Luka Image!",
                Description = $"[Full Source Image Link]({myresponse.url.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            if (myresponse.creator.Length != 0)
            {
                emim.AddField("Creator", myresponse.creator);
            }
            response.Close();
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);

            await ctx.RespondWithFileAsync(fileName: $"image.{GetExtension(response2.ContentType)}", fileData: dataStream2, embed: emim.Build());
        }

        [Command("ia"), Description("Shows you a IA image")]
        public async Task IAPic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("https://api.meek.moe/ia");
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
                Title = "Random IA Image!",
                Description = $"[Full Source Image Link]({myresponse.url.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            if (myresponse.creator.Length != 0)
            {
                emim.AddField("Creator", myresponse.creator);
            }
            response.Close();
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);

            await ctx.RespondWithFileAsync(fileName: $"image.{GetExtension(response2.ContentType)}", fileData: dataStream2, embed: emim.Build());
        }

        [Command("yukari"), Description("Shows you a Yukari image")]
        public async Task YYukariPic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("https://api.meek.moe/yukari");
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
                Title = "Random Yukari Image!",
                Description = $"[Full Source Image Link]({myresponse.url.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            if (myresponse.creator.Length != 0)
            {
                emim.AddField("Creator", myresponse.creator);
            }
            response.Close();
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);

            await ctx.RespondWithFileAsync(fileName: $"image.{GetExtension(response2.ContentType)}", fileData: dataStream2, embed: emim.Build());
        }

        [Command("teto"), Description("Shows you a Teto image")]
        public async Task KTetoPic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("https://api.meek.moe/teto");
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
                Title = "Random Teto Image!",
                Description = $"[Full Source Image Link]({myresponse.url.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            if (myresponse.creator.Length != 0)
            {
                emim.AddField("Creator", myresponse.creator);
            }
            response.Close();
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);

            await ctx.RespondWithFileAsync(fileName: $"image.{GetExtension(response2.ContentType)}", fileData: dataStream2, embed: emim.Build());
        }

        [Command("len"), Description("Shows you a Len image")]
        public async Task KLenPic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("https://api.meek.moe/len");
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
                Title = "Random Len Image!",
                Description = $"[Full Source Image Link]({myresponse.url.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            if (myresponse.creator.Length != 0)
            {
                emim.AddField("Creator", myresponse.creator);
            }
            response.Close();
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);

            await ctx.RespondWithFileAsync(fileName: $"image.{GetExtension(response2.ContentType)}", fileData: dataStream2, embed: emim.Build());
        }

        [Command("kaito"), Description("Shows you a Kaito image")]
        public async Task KaitoPic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("https://api.meek.moe/kaito");
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
                Title = "Random Kaito Image!",
                Description = $"[Full Source Image Link]({myresponse.url.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            if (myresponse.creator.Length != 0)
            {
                emim.AddField("Creator", myresponse.creator);
            }
            response.Close();
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);

            await ctx.RespondWithFileAsync(fileName: $"image.{GetExtension(response2.ContentType)}", fileData: dataStream2, embed: emim.Build());
        }

        [Command("meiko"), Description("Shows you a Meiko image")]
        public async Task MeikoPic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("https://api.meek.moe/meiko");
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
                Title = "Random Meiko Image!",
                Description = $"[Full Source Image Link]({myresponse.url.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            if (myresponse.creator.Length != 0)
            {
                emim.AddField("Creator", myresponse.creator);
            }
            response.Close();
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);

            await ctx.RespondWithFileAsync(fileName: $"image.{GetExtension(response2.ContentType)}", fileData: dataStream2, embed: emim.Build());
        }

        [Command("fukase"), Description("Shows you a Meiko image")]
        public async Task FukasePic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("https://api.meek.moe/fukase");
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
                Title = "Random Fukase Image!",
                Description = $"[Full Source Image Link]({myresponse.url.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            if (myresponse.creator.Length != 0)
            {
                emim.AddField("Creator", myresponse.creator);
            }
            response.Close();
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);

            await ctx.RespondWithFileAsync(fileName: $"image.{GetExtension(response2.ContentType)}", fileData: dataStream2, embed: emim.Build());
        }

        [Command("miku"), Description("Shows you a Meiko image")]
        public async Task HMikuPic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("https://api.meek.moe/miku");
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
                Title = "Random Miku Image!",
                Description = $"[Full Source Image Link]({myresponse.url.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            if (myresponse.creator.Length != 0)
            {
                emim.AddField("Creator", myresponse.creator);
            }
            response.Close();
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);

            await ctx.RespondWithFileAsync(fileName: $"image.{GetExtension(response2.ContentType)}", fileData: dataStream2, embed: emim.Build());
        }

        [Command("miki"), Description("Shows you a Meiko image")]
        public async Task SFMikiPic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("https://api.meek.moe/miki");
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
                Title = "Random Miki Image!",
                Description = $"[Full Source Image Link]({myresponse.url.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            if (myresponse.creator.Length != 0)
            {
                emim.AddField("Creator", myresponse.creator);
            }
            response.Close();
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);

            await ctx.RespondWithFileAsync(fileName: $"image.{GetExtension(response2.ContentType)}", fileData: dataStream2, embed: emim.Build());
        }

        [Command("mayu"), Description("Shows you a Meiko image")]
        public async Task MayuPic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("https://api.meek.moe/mayu");
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
                Title = "Random Mayu Image!",
                Description = $"[Full Source Image Link]({myresponse.url.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            if (myresponse.creator.Length != 0)
            {
                emim.AddField("Creator", myresponse.creator);
            }
            response.Close();
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);

            await ctx.RespondWithFileAsync(fileName: $"image.{GetExtension(response2.ContentType)}", fileData: dataStream2, embed: emim.Build());
        }

        [Command("aoki"), Description("Shows you a Meiko image")]
        public async Task LAokiPic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("https://api.meek.moe/aoki");
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
                Title = "Random Aoki Image!",
                Description = $"[Full Source Image Link]({myresponse.url.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            if (myresponse.creator.Length != 0)
            {
                emim.AddField("Creator", myresponse.creator);
            }
            response.Close();
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);

            await ctx.RespondWithFileAsync(fileName: $"image.{GetExtension(response2.ContentType)}", fileData: dataStream2, embed: emim.Build());
        }

        [Command("lily"), Description("Shows you a Meiko image")]
        public async Task LilyPic(CommandContext ctx)
        {
            WebRequest request = WebRequest.Create("https://api.meek.moe/lily");
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
                Title = "Random Lily Image!",
                Description = $"[Full Source Image Link]({myresponse.url.ToString()})",
                ImageUrl = $"attachment://image.{GetExtension(response2.ContentType)}"
            };
            if (myresponse.creator.Length != 0)
            {
                emim.AddField("Creator", myresponse.creator);
            }
            response.Close();
            emim.WithAuthor(name: "via api.meek.moe", url: "https://api.meek.moe/");
            emim.WithFooter("Requested by " + ctx.Message.Author.Username, ctx.Message.Author.AvatarUrl);

            await ctx.RespondWithFileAsync(fileName: $"image.{GetExtension(response2.ContentType)}", fileData: dataStream2, embed: emim.Build());
        }

        public class ImgRet
        {
            [JsonProperty("url")]
            public string url { get; set; }
            [JsonProperty("creator")]
            public string creator { get; set; }
        }
    }
}
