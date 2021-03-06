﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Nayu.Libs.CustomLibraries.Discord.Addons.Interactive.Criteria;
using Nayu.Libs.CustomLibraries.Discord.Addons.Interactive.Paginator;
using Nayu.Libs.CustomLibraries.Discord.Addons.Interactive.Results;

namespace Nayu.Libs.CustomLibraries.Discord.Addons.Interactive
{
    public class InteractiveBase : InteractiveBase<ShardedCommandContext>
    {
    }

    public class InteractiveBase<T> : ModuleBase<T>
        where T : ShardedCommandContext
    {
        public InteractiveService Interactive { get; set; }

        public Task<SocketMessage> NextMessageAsync(ICriterion<SocketMessage> criterion, TimeSpan? timeout = null)
            => Interactive.NextMessageAsync(Context, criterion, timeout);
        public Task<SocketMessage> NextMessageAsync(bool fromSourceUser = true, bool inSourceChannel = true, TimeSpan? timeout = null) 
            => Interactive.NextMessageAsync(Context, fromSourceUser, inSourceChannel, timeout);

        public Task<IUserMessage> ReplyAndDeleteAsync(string content, bool isTTS = false, Embed embed = null, TimeSpan? timeout = null, RequestOptions options = null)
            => Interactive.ReplyAndDeleteAsync(Context, content, isTTS, embed, timeout, options);

        public Task<IUserMessage> PagedReplyAsync(IEnumerable<object> pages, bool fromSourceUser = true)
        {
            var pager = new PaginatedMessage
            {
                Pages = pages
            };
            return PagedReplyAsync(pager, fromSourceUser);
        }
        public Task<IUserMessage> PagedReplyAsync(PaginatedMessage pager, bool fromSourceUser = true)
        {
            var criterion = new Criteria<SocketReaction>();
            if (fromSourceUser)
                criterion.AddCriterion(new EnsureReactionFromSourceUserCriterion());
            return PagedReplyAsync(pager, criterion);
        }
        public Task<IUserMessage> PagedReplyAsync(PaginatedMessage pager, ICriterion<SocketReaction> criterion)
            => Interactive.SendPaginatedMessageAsync(Context, pager, criterion);

        public RuntimeResult Ok(string reason = null) => new OkResult(reason);
    }
}
