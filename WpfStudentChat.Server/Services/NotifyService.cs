using LibStudentChat.Models;
using WpfStudentChat.Server.Utilities;
using CommonModels = LibStudentChat.Models;

namespace WpfStudentChat.Server.Services
{
    public class NotifyService
    {
        public event AsyncEventHandler<PrivateMessageSentEventArgs>? PrivateMessageSent;
        public event AsyncEventHandler<GroupMessageSentEventArgs>? GroupMessageSent;
        public event AsyncEventHandler<FriendRequestSentEvnetArgs>? FriendRequestSent;
        public event AsyncEventHandler<GroupRequestSentEvnetArgs>? GroupRequestSent;

        public event AsyncEventHandler<FriendChangedEventArgs>? FriendIncreased;
        public event AsyncEventHandler<FriendChangedEventArgs>? FriendDecreased;
        public event AsyncEventHandler<GroupChangedEventArgs>? GroupIncreased;
        public event AsyncEventHandler<GroupChangedEventArgs>? GroupDecreased;


        public async Task OnPrivateMessageSent(CommonModels.PrivateMessage message)
        {
            if (PrivateMessageSent is null)
                return;

            await PrivateMessageSent.Invoke(this, new PrivateMessageSentEventArgs(message));
        }

        public async Task OnGroupMessageSent(CommonModels.GroupMessage message)
        {
            if (GroupMessageSent is null)
                return;

            await GroupMessageSent.Invoke(this, new GroupMessageSentEventArgs(message));
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
}
