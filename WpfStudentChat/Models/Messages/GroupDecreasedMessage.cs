using StudentChat.Models;

namespace WpfStudentChat.Models.Messages
{
    public class GroupDecreasedMessage(Group group)
    {
        public Group Group { get; } = group;
    }
}
