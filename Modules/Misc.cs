using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Net;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace CaligulaLite.Modules
{
    public class MiscCommands : ModuleBase
    {
        [Command("shoot")]
        public async Task HelloCommand(Discord.IUser user)
        {
            var sb = new StringBuilder();
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name == "Wounded");
            sb.AppendLine($"Pow, {user.Mention}");

            await ReplyAsync(sb.ToString());
            await Context.Channel.SendFileAsync("Assets/beastpow.png");
            await (user as IGuildUser).AddRoleAsync(role);
        }
    }
}