using StudentChat.Models;

namespace WpfStudentChat.Models.Messages
{
    public class GroupMessageReceivedMessage(GroupMessage message)
    {
        public GroupMessage Message { get; } = message;
    }
}
