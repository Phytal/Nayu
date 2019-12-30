﻿using Discord;
using Discord.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nayu.Libs.Weeb.net;
using Nayu.Libs.Weeb.net.Data;
using Nayu.Preconditions;

namespace Nayu.Modules.API.Anime.weebDotSh
{
    public class Megumin : NayuModule
    {
        [Command("megumin")]
        [Summary("Displays a Megumin image/gif")]
        [Remarks("Ex: n!megumin")]
        [Cooldown(5)]
        public async Task LewdIMG()
        {
            string[] tags = new[] { "" };
            Helpers.WebRequest webReq = new Helpers.WebRequest();
            RandomData result = await webReq.GetTypesAsync("megumin", tags, FileType.Any, NsfwSearch.False, false);
            string url = result.Url;
            string id = result.Id;
            var embed = new EmbedBuilder();

            embed.WithColor(37, 152, 255);
            embed.WithTitle("Explosion!");
            embed.WithDescription(
                $"{Context.User.Mention} here's some megumin pics at your disposal :3");
            embed.WithImageUrl(url);
            embed.WithFooter($"Powered by weeb.sh | ID: {id}");

            await Context.Channel.SendMessageAsync("", embed: embed.Build());

        }
    }
}
