﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Discord.Commands;
using Discord;
using System.Net;
using Nayu.Helpers;
using Nayu.Preconditions;

namespace Nayu.Modules.API
{
    public class UrbanDictionary : NayuModule
    {
        [Subject(Categories.Fun)]
        [Command("define")]
        [Summary("Use Urban Dictionary to define a given word")]
        [Alias("dictionary", "urban", "definition")]
        [Remarks("n!define <word you want to define> Ex: n!define Weeb")]
        [Cooldown(5)]
        public async Task Define([Remainder] string link)
        {
            if (Context.Guild.Id == 264445053596991498 && !(Context.Channel as ITextChannel).IsNsfw
            ) //dbl server (dont kill me pls)
                return;

            string json;
            using (WebClient client = new WebClient())
            {
                json = client.DownloadString("http://api.urbandictionary.com/v0/define?term=" + link);
            }

            var dataObject = JsonConvert.DeserializeObject<dynamic>(json);

            string author = dataObject.list[0].author.ToString();
            string definition = dataObject.list[0].definition.ToString();
            string example = dataObject.list[0].example.ToString();
            string permalink = dataObject.list[0].permalink.ToString();
            string up = dataObject.list[0].thumbs_up.ToString();
            string down = dataObject.list[0].thumbs_down.ToString();

            var embed = new EmbedBuilder();
            embed.WithColor(Global.NayuColor);
            embed.WithTitle(link);
            embed.WithDescription($"By *{author}*");
            embed.AddField("Definition", definition, true);
            embed.AddField("Example", example + "\n" +
                                      "\n:thumbsup:" + up + " :thumbsdown:" + down, true);
            embed.WithFooter("Provided by the Urban Dictionary API");
            embed.WithUrl(permalink);

            await SendMessage(Context, embed.Build());
        }
    }
}