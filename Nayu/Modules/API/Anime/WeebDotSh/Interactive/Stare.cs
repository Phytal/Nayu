﻿using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Nayu.Helpers;
using Nayu.Libs.Weeb.net;
using Nayu.Libs.Weeb.net.Data;
using Nayu.Preconditions;

namespace Nayu.Modules.API.Anime.WeebDotSh.Interactive
{
    public class Stare : NayuModule
    {
        [Subject(Categories.Interaction)]
        [Command("stare")]
        [Summary("Displays an image of an anime stare gif")]
        [Remarks("Usage: n!stare <user you want to bite (or can be left empty)> Ex: n!stare @Phytal")]
        [Cooldown(5)]
        public async Task StareUser(IGuildUser user = null)
        {
            string[] tags = {""};
            Helpers.WebRequest webReq = new Helpers.WebRequest();
            RandomData result = await webReq.GetTypesAsync("stare", tags, FileType.Gif, NsfwSearch.False, false);
            string url = result.Url;
            string id = result.Id;
            if (user == null)
            {
                var embed = new EmbedBuilder();
                embed.WithColor(Global.NayuColor);
                embed.WithTitle("Chiiiiii...");
                embed.WithDescription(
                    $"{Context.User.Mention} is staring at themselves! You know, you can't win a staring contest against a mirror.. \n**(Include a user with your command! Example: n!stare <person you want to stare at>)**");
                embed.WithImageUrl(url);
                embed.WithFooter($"Powered by weeb.sh | ID: {id}");

                await SendMessage(Context, embed.Build());
            }
            else
            {
                var embed = new EmbedBuilder();
                embed.WithColor(Global.NayuColor);
                embed.WithImageUrl(url);
                embed.WithTitle("Chiiiiii...");
                embed.WithDescription($"{Context.User.Username} is staring at {user.Mention}!");
                embed.WithFooter($"Powered by weeb.sh | ID: {id}");

                await SendMessage(Context, embed.Build());
            }
        }
    }
}