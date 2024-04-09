using StudentChat.Models;

namespace WpfStudentChat.Models.Messages
{
    public class GroupIncreasedMessage(Group group)
    {
        public Group Group { get; } = group;
    }
}
