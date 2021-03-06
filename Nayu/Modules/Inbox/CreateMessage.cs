﻿using System;
using System.Threading.Tasks;
using Discord.WebSocket;
using Nayu.Core.Entities;
using Nayu.Core.Features.GlobalAccounts;

namespace Nayu.Modules.Inbox
{
    public class CreateMessage : NayuModule
    {
        public static async Task CreateAndSendMessageAsync(string title, string content, DateTime time, SocketUser user)
        {
            var config = GlobalUserAccounts.GetUserAccount(user);
            Message createdMessage = new Message(null, null, DateTime.MinValue, false, 0);
            createdMessage.Title = title;
            createdMessage.Content = content;
            createdMessage.Time = time;
            createdMessage.ID = config.InboxIDTracker;
            config.InboxIDTracker++;
            config.Inbox.Add(createdMessage);
            GlobalUserAccounts.SaveAccounts(user.Id);
        }
    }
}