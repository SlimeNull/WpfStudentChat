using StudentChat.Models;

namespace WpfStudentChat.Models.Messages
{
    public class GroupRequestReceivedMessage(GroupRequest request)
    {
        public GroupRequest Request { get; } = request;
    }
}
