﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Nayu.Core.Handlers;
using Nayu.Helpers;
using Nayu.Preconditions;

namespace Nayu.Modules.API.Anime.NekosLife.NSFWHentai
{
    public class Spank : NayuModule
    {
        [Subject(NSFWCategories.Hentai)]
        [Command("spank")]
        [Summary("Displays a spank")]
        [Remarks("Ex: n!spank")]
        [Cooldown(5)]
        public async Task GetRandomErofeetSpank()
        {
            var channel = Context.Channel as ITextChannel;
            if (!channel.IsNsfw)
            {
                var nsfwText =
                    $"{Global.ENo} **|** You need to use this command in a NSFW channel, {Context.User.Username}!";
                var errorEmbed = EmbedHandler.CreateEmbed(Context, "Error", nsfwText,
                    EmbedHandler.EmbedMessageType.Exception);
                await ReplyAndDeleteAsync("", embed: errorEmbed);
                return;
            }

            string nekoLink = NekosLifeHelper.GetNekoLink("spank");
            var title = "Randomly generated spank just for you <3!";
            var embed = ImageEmbed.GetImageEmbed(nekoLink, Source.NekosLife, title);
            await SendMessage(Context, embed);
        }
    }
}