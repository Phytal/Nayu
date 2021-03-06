﻿using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Nayu.Helpers;
using Nayu.Libs.Weeb.net;
using Nayu.Libs.Weeb.net.Data;
using Nayu.Preconditions;

namespace Nayu.Modules.API.Anime.WeebDotSh.Interactive
{
    public class Shrug : NayuModule
    {
        [Subject(Categories.Interaction)]
        [Command("shrug")]
        [Summary("Displays an image of an anime shrug gif")]
        [Remarks("Usage: n!shrug <user you want to shrug at (or can be left empty)> Ex: n!shrug @Phytal")]
        [Cooldown(5)]
        public async Task ShrugUser(IGuildUser user = null)
        {
            string[] tags = {""};
            Helpers.WebRequest webReq = new Helpers.WebRequest();
            RandomData result = await webReq.GetTypesAsync("shrug", tags, FileType.Gif, NsfwSearch.False, false);
            string url = result.Url;
            string id = result.Id;
            if (user == null)
            {
                var embed = new EmbedBuilder();
                embed.WithColor(Global.NayuColor);
                embed.WithTitle("Shrug!");
                embed.WithDescription(
                    $"{Context.User.Mention} shrugged at themselves... I wonder what {Context.User.Username} is thinking about.. \n**(Include a user with your command! Example: n!shrug <person you want to shrug>)**");
                embed.WithImageUrl(url);
                embed.WithFooter($"Powered by weeb.sh | ID: {id}");

                await SendMessage(Context, embed.Build());
            }
            else
            {
                var embed = new EmbedBuilder();
                embed.WithColor(Global.NayuColor);
                embed.WithImageUrl(url);
                embed.WithTitle("Shrug!");
                embed.WithDescription($"{Context.User.Username} shrugged at {user.Mention}!");
                embed.WithFooter($"Powered by weeb.sh | ID: {id}");

                await SendMessage(Context, embed.Build());
            }
        }
    }
}