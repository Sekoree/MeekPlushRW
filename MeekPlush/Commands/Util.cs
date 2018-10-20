using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeekPlush.Commands
{
    [Group("util")]
    class Util : BaseCommandModule
    {
        [Command("getroleid"),Aliases("rid")]
        public async Task GetRoleID(CommandContext ctx, string name)
        {
            var inter = ctx.Client.GetInteractivity();
            var roles = ctx.Guild.Roles.Where(x => x.Name.Contains(name));
            var emb = new DiscordEmbedBuilder();
            int pi = 1;
            List<Page> rL = new List<Page>();
            var Grole = await ctx.Guild.GetAllMembersAsync();
            if (roles.Count() == 1) {
                var Mcount = Grole.Where(x => x.Roles.Any(y => y.Id == roles.First().Id)).Count();
                emb.WithTitle(roles.First().Name);
                emb.WithDescription($"ID: **{roles.First().Id}**\n" +
                    $"Mentionable: **{roles.First().IsMentionable}**\n" +
                    $"Position: **{roles.First().Position}**\n" +
                    $"Member Count: **{Mcount}**\n" +
                    $"Color: **{roles.First().Color.Value}**");
                emb.WithFooter($"Requested by {ctx.Member.DisplayName ?? ctx.Member.Username}", ctx.User.AvatarUrl);
                rL.Add(new Page {
                    Embed = emb.Build()
                });
                await ctx.RespondAsync(embed: rL.First().Embed);
                return;
            } else {
                foreach (var r in roles) {
                    var Mcount = Grole.Where(x => x.Roles.Any(y => y.Id == r.Id)).Count();
                    emb.WithTitle(r.Name);
                    emb.WithDescription($"ID: **{r.Id}**\n" +
                        $"Mentionable: **{r.IsMentionable}**\n" +
                        $"Position: **{r.Position}**\n" +
                        $"Member Count: **{Mcount}**\n" +
                        $"Color: **{r.Color.Value}**");
                    emb.WithFooter($"Page {pi}/{roles.Count()} || Requested by {ctx.Member.DisplayName ?? ctx.Member.Username}", ctx.User.AvatarUrl);
                    rL.Add(new Page {
                        Embed = emb.Build()
                    });
                    pi++;
                }
            }
            await inter.SendPaginatedMessage(ctx.Channel, ctx.User, rL);
        }
    }
}
