using StudentChat.Models;

namespace WpfStudentChat.Models.Messages
{
    public class FriendIncreasedMessage(User friend)
    {
        public User Friend { get; } = friend;
    }
}
