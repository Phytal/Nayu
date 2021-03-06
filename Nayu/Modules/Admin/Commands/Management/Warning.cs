﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Nayu.Core.Features.GlobalAccounts;
using Nayu.Core.Handlers;
using Nayu.Helpers;
using Nayu.Preconditions;

namespace Nayu.Modules.Admin.Commands.Management
{
    public class Warning : NayuModule
    {
        [Subject(AdminCategories.UserManagement)]
        [Command("Warn")]
        [Summary("Warns a User")]
        [Remarks("n!carn <user you want to warn> <reason> Ex: n!carn @Phytal bullied my brother")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [Cooldown(5)]
        public async Task WarnUser([NoSelf] IGuildUser user, [Remainder] string reason = "No reason provided.")
        {
            var guildUser = Context.User as SocketGuildUser;
            if (!guildUser.GuildPermissions.ManageMessages)
            {
                string description =
                    $"{Global.ENo} **|** You Need the **Manage Messages** Permission to do that {Context.User.Username}";
                var errorEmbed = EmbedHandler.CreateEmbed(Context, "Error", description,
                    EmbedHandler.EmbedMessageType.Exception);
                await ReplyAndDeleteAsync("", embed: errorEmbed);
                return;
            }

            if (user == null)
            {
                var embed = EmbedHandler.CreateEmbed(Context, "Error",
                    "🖐️ **|** Please say who you want to warn and a reason for their warning. Ex: n!warn @Phytal bullied my brother",
                    EmbedHandler.EmbedMessageType.Exception);
                await ReplyAndDeleteAsync("", embed: embed);
            }

            var userAccount = GlobalGuildUserAccounts.GetUserID((SocketGuildUser) user);
            var dmChannel = await user.GetOrCreateDMChannelAsync();
            try
            {
                var dmDesc = $"You have been warned in **{Context.Guild.Name}** for:\n {reason}\nYou now have Warning{(userAccount.NumberOfWarnings == 1 ? "" : "s")}";
                var dmEmbed = EmbedHandler.CreateEmbed(Context, "Warning", dmDesc, EmbedHandler.EmbedMessageType.Info, false);
                await dmChannel.SendMessageAsync("", embed: dmEmbed);
            }
            catch (Exception)
            {
                //nothing should happen and it doesn't matter if anything does happen
            }
            userAccount.NumberOfWarnings++;
            userAccount.Warnings.Add(reason);
            GlobalGuildUserAccounts.SaveAccounts();
            
            var desc = $"Successfully warned **{user.Username}** for **{reason}**. **({userAccount.NumberOfWarnings} Warning{(userAccount.NumberOfWarnings == 1 ? "" : "s")})**";
            var successEmbed = EmbedHandler.CreateEmbed(Context, "Warning", desc, EmbedHandler.EmbedMessageType.Success);
            await SendMessage(Context, successEmbed);
        }

        [Subject(AdminCategories.UserManagement)]
        [Command("Warnings")]
        [Summary("Shows all of a user's warnings")]
        [Remarks("n!warnings <user whose warnings you want to look at> Ex: n!warnings @Phytal")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [Cooldown(5)]
        public async Task Warnings(IGuildUser user)
        {
            var guildUser = Context.User as SocketGuildUser;
            if (!guildUser.GuildPermissions.ManageMessages)
            {
                string description =
                    $"{Global.ENo} **|** You Need the **Manage Messages** Permission to do that {Context.User.Username}";
                var errorEmbed = EmbedHandler.CreateEmbed(Context, "Error", description,
                    EmbedHandler.EmbedMessageType.Exception);
                await ReplyAndDeleteAsync("", embed: errorEmbed);
                return;
            }

            var num = GlobalGuildUserAccounts.GetUserID((SocketGuildUser) user).NumberOfWarnings;
            var warnings = GlobalGuildUserAccounts.GetUserID((SocketGuildUser) user).Warnings;
            var embed = new EmbedBuilder();
            embed.WithTitle($"{user}'s Warnings");
            embed.WithDescription($"Total of **{num}** warnings");
            for (var i = 0; i < warnings.Count; i++)
            {
                embed.AddField($"Warning #{i + 1}: ", warnings[i], true);
            }

            await SendMessage(Context, embed.Build());
        }

        [Subject(AdminCategories.UserManagement)]
        [Command("ClearWarnings")]
        [Summary("Clears all of a user's warnings")]
        [Alias("cw")]
        [Remarks("n!cw <user whose warnings you want to clear> Ex: n!cw @Phytal")]
        [RequireBotPermission(GuildPermission.BanMembers)]
        [Cooldown(5)]
        public async Task ClearWarnings(IGuildUser user)
        {
            var guildUser = Context.User as SocketGuildUser;
            if (!guildUser.GuildPermissions.ManageMessages)
            {
                string description =
                    $"{Global.ENo} **|** You Need the **Manage Messages** Permission to do that {Context.User.Username}";
                var errorEmbed = EmbedHandler.CreateEmbed(Context, "Error", description,
                    EmbedHandler.EmbedMessageType.Exception);
                await ReplyAndDeleteAsync("", embed: errorEmbed);
                return;
            }

            var userAccount = GlobalGuildUserAccounts.GetUserID((SocketGuildUser) user);
            userAccount.NumberOfWarnings = 0;
            userAccount.Warnings.Clear();
            GlobalGuildUserAccounts.SaveAccounts();

            await SendMessage(Context, null, $"✅  Succesfully cleared all of **{user.Username}'s** warnings.");
        }
    }
}