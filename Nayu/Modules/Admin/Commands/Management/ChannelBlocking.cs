﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Nayu.Core.Features.GlobalAccounts;
using Nayu.Helpers;
using Nayu.Preconditions;

namespace Nayu.Modules.Admin.Commands.Management
{
    public class ChannelBlocking : NayuModule
    {
        [Command("blockchannel"), Alias("bc")]
        [Summary("Blocks the current channel (users will be unable to use commands, only admins)")]
        [Remarks("n!bc")]
        [Cooldown(5)]
        public async Task BlockChannel()
        {
            var guildUser = Context.User as SocketGuildUser;
            if (guildUser.GuildPermissions.ManageChannels)
            {
                var config = BotAccounts.GetAccount();
                config.BlockedChannels.Add(Context.Channel.Id, Context.Guild.Id);
                BotAccounts.SaveAccounts();

                var embed = MiscHelpers.CreateEmbed(Context, "Channel Blocked", $":lock: Blocked {Context.Channel.Name}.");
                await MiscHelpers.SendMessage(Context, embed);
            }
            else
            {
                throw new Exception(":x:  | You Need the Manage Channels Permission to do that {Context.User.Username}");
            }
        }

        [Command("unblockchannel"), Alias("ubc")]
        [Summary("Unblocks the current channel (users can use commands again)")]
        [Remarks("n!ubc")]
        [Cooldown(5)]
        public async Task UnblockChannel()
        {
            var guildUser = Context.User as SocketGuildUser;
            if (guildUser.GuildPermissions.ManageChannels)
            {
                var config = BotAccounts.GetAccount();
                config.BlockedChannels.Remove(Context.Channel.Id);
                BotAccounts.SaveAccounts();

                var embed = MiscHelpers.CreateEmbed(Context, "Channel Unblocked", $":unlock: Unblocked {Context.Channel.Name}.").WithColor(Constants.DefaultColor);
                await MiscHelpers.SendMessage(Context, embed);
            }
            else
            {
                throw new Exception(":x:  | You Need the Manage Channels Permission to do that {Context.User.Username}");
            }
        }
    }
}