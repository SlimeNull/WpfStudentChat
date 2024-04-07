using LibStudentChat.Models;
using WpfStudentChat.Server.Utilities;
using CommonModels = LibStudentChat.Models;

namespace WpfStudentChat.Server.Services
{
    public class MessageNotifyService
    {
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

        public event AsyncEventHandler<PrivateMessageSentEventArgs>? PrivateMessageSent;
        public event AsyncEventHandler<GroupMessageSentEventArgs>? GroupMessageSent;

        public class PrivateMessageSentEventArgs : EventArgs
        {
            public PrivateMessageSentEventArgs(CommonModels.PrivateMessage message)
            {
                Message = message;
            }

            public PrivateMessage Message { get; }
        }

        public class GroupMessageSentEventArgs : EventArgs
        {
            public GroupMessageSentEventArgs(CommonModels.GroupMessage message)
            {
                Message = message;
            }

            public GroupMessage Message { get; }
        }

    }
}
