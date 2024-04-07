using LibStudentChat.Models;
using CommonModels = LibStudentChat.Models;

namespace WpfStudentChat.Server.Services
{
    public class MessageNotifyService
    {
        public void OnPrivateMessageSent(CommonModels.PrivateMessage message)
        {
            PrivateMessageSent?.Invoke(this, new PrivateMessageSentEventArgs(message));
        }

        public void OnGroupMessageSent(CommonModels.GroupMessage message)
        {
            GroupMessageSent?.Invoke(this, new GroupMessageSentEventArgs(message));
        }

        public event EventHandler<PrivateMessageSentEventArgs>? PrivateMessageSent;
        public event EventHandler<GroupMessageSentEventArgs>? GroupMessageSent;

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
