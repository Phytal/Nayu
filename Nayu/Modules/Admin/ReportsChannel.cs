﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Nayu.Core.Handlers;
using Nayu.Helpers;
using Nayu.Preconditions;

namespace Nayu.Modules.Admin
{
    public class ReportsChannel : NayuModule
    {
        [Subject(AdminCategories.ServerManagement)]
        [Command("reports")]
        [Summary("If the reports channel isn't automatically created, you can use this command to manually create it")]
        [Remarks("Ex: n!reports")]
        [Cooldown(5)]
        public async Task Text()
        {
            var guildUser = Context.User as SocketGuildUser;
            if (!guildUser.GuildPermissions.ManageChannels)
            {
                string description =
                    $"{Global.ENo} **|** You Need the **Manage Channels** Permission to do that {Context.User.Username}";
                var errorEmbed = EmbedHandler.CreateEmbed(Context, "Error", description,
                    EmbedHandler.EmbedMessageType.Exception);
                await ReplyAndDeleteAsync("", embed: errorEmbed);
                return;
            }

            var role = Context.Guild.Roles.FirstOrDefault(r => r.Name == "@everyone");
            var perms = new OverwritePermissions(
                sendMessages: PermValue.Deny,
                addReactions: PermValue.Deny,
                viewChannel: PermValue.Allow
            );
            var channel = await Context.Guild.CreateTextChannelAsync("Reports");
            await channel.AddPermissionOverwriteAsync(role, perms);
        }

        [Subject(Categories.Other)]
        [Command("report")]
        [Summary("Reports @Username")]
        [Remarks("n!report <user> <reason> Ex: n!report @Phytal abusing")]
        [Cooldown(5)]
        public async Task ReportAsync([NoSelf] SocketGuildUser user, [Remainder] string reason)
        {
            if ((user == null)
                || (string.IsNullOrWhiteSpace(reason)))
            {
                var embed = new EmbedBuilder();
                embed.WithColor(Global.NayuColor);
                embed.WithTitle(
                    "🖐️ **|** You must mention a user and provide a reason. Ex: n!report @Username <reason>");
                await ReplyAndDeleteAsync("", embed: embed.Build(), timeout: TimeSpan.FromSeconds(5));
            }
            else
            {
                var chnl = Context.Guild.TextChannels.FirstOrDefault(r => r.Name == "reports");
                if (chnl == null)
                {
                    var role = Context.Guild.Roles.FirstOrDefault(r => r.Name == "@everyone");
                    var perms = new OverwritePermissions(
                        sendMessages: PermValue.Deny,
                        addReactions: PermValue.Deny,
                        viewChannel: PermValue.Allow
                    );
                    var channell = await Context.Guild.CreateTextChannelAsync("reports");
                    await channell.AddPermissionOverwriteAsync(role, perms);
                }

                var channel = chnl as SocketTextChannel;
                var embed = new EmbedBuilder();
                embed.WithColor(Global.NayuColor);
                embed.Title = $"{Context.User}'s report of {user.Username}";
                embed.Description =
                    $"**Username: **{user.Username}\n**Reported by: **{Context.User.Mention}\n**Reason: **{reason}";
                await SendMessage(Context, embed.Build());
                await ReplyAsync("✅  **|** *Your report has been furthered to staff.*");
            }
        }

        public async Task GetReportChannel(IGuild guild)
        {
            var channelName = "Reports";

            var TextChannel = Context.Guild.TextChannels.FirstOrDefault(r => r.Name == channelName);

            if (TextChannel == null)
            {
                var role = Context.Guild.Roles.FirstOrDefault(r => r.Name == "@everyone");
                var perms = new OverwritePermissions(
                    sendMessages: PermValue.Deny,
                    addReactions: PermValue.Deny,
                    viewChannel: PermValue.Allow
                );
                var channel = await Context.Guild.CreateTextChannelAsync("Reports");
                await channel.AddPermissionOverwriteAsync(role, perms);
            }
        }
    }
}