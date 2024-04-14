using StudentChat.Models;
using StudentChat.Server.Utilities;
using CommonModels = StudentChat.Models;

namespace StudentChat.Server.Services;

public class NotifyService
{
    public AsyncEventHandler<PrivateMessageSentEventArgs>? PrivateMessageSent { get; set; } = null;
    public AsyncEventHandler<GroupMessageSentEventArgs>? GroupMessageSent { get; set; } = null;
    public AsyncEventHandler<FriendRequestSentEvnetArgs>? FriendRequestSent { get; set; } = null;
    public AsyncEventHandler<GroupRequestSentEvnetArgs>? GroupRequestSent { get; set; } = null;

    public AsyncEventHandler<FriendChangedEventArgs>? FriendIncreased { get; set; } = null;
    public AsyncEventHandler<FriendChangedEventArgs>? FriendDecreased { get; set; } = null;
    public AsyncEventHandler<GroupChangedEventArgs>? GroupIncreased { get; set; } = null;
    public AsyncEventHandler<GroupChangedEventArgs>? GroupDecreased { get; set; } = null;


    public async Task OnPrivateMessageSent(CommonModels.PrivateMessage message)
    {
        if (PrivateMessageSent is null)
            return;

        await PrivateMessageSent.InvokeAsync(this, new PrivateMessageSentEventArgs(message));
    }

    public async Task OnGroupMessageSent(CommonModels.GroupMessage message)
    {
        if (GroupMessageSent is null)
            return;

        await GroupMessageSent.InvokeAsync(this, new GroupMessageSentEventArgs(message));
    }

    public async Task OnFriendRequestSent(CommonModels.FriendRequest request)
    {
        if (FriendRequestSent is null)
            return;

        await FriendRequestSent.InvokeAsync(this, new FriendRequestSentEvnetArgs(request));
    }

    public async Task OnGroupRequestSent(CommonModels.GroupRequest request)
    {
        if (GroupRequestSent is null)
            return;

        await GroupRequestSent.InvokeAsync(this, new GroupRequestSentEvnetArgs(request));
    }

    public async Task OnFriendIncreased(int userId, CommonModels.User friend)
    {
        if (FriendIncreased is null)
            return;

        await FriendIncreased.InvokeAsync(this, new FriendChangedEventArgs(userId, friend));
    }

    public async Task OnFriendDecreased(int userId, CommonModels.User friend)
    {
        if (FriendDecreased is null)
            return;

        await FriendDecreased.InvokeAsync(this, new FriendChangedEventArgs(userId, friend));
    }

    public async Task OnGroupIncreased(int userId, CommonModels.Group group)
    {
        if (GroupIncreased is null)
            return;

        await GroupIncreased.InvokeAsync(this, new GroupChangedEventArgs(userId, group));
    }

    public async Task OnGroupDecreased(int userId, CommonModels.Group group)
    {
        if (GroupDecreased is null)
            return;

        await GroupDecreased.InvokeAsync(this, new GroupChangedEventArgs(userId, group));
    }


    public class PrivateMessageSentEventArgs(CommonModels.PrivateMessage message) : EventArgs
    {
        public PrivateMessage Message { get; } = message;
    }

    public class GroupMessageSentEventArgs(CommonModels.GroupMessage message) : EventArgs
    {
        public GroupMessage Message { get; } = message;
    }

    public class FriendRequestSentEvnetArgs(CommonModels.FriendRequest request) : EventArgs
    {
        public FriendRequest Request { get; } = request;
    }

    public class GroupRequestSentEvnetArgs(CommonModels.GroupRequest request) : EventArgs
    {
        public GroupRequest Request { get; } = request;
    }

    public class FriendChangedEventArgs(int userId, CommonModels.User friend) : EventArgs
    {
        public int UserId { get; } = userId;
        public User Friend { get; } = friend;
    }

    public class GroupChangedEventArgs(int userId, CommonModels.Group group) : EventArgs
    {
        public int UserId { get; } = userId;
        public Group Group { get; } = group;
    }
}
