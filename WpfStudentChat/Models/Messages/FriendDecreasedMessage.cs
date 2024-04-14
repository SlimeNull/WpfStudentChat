using StudentChat.Models;

namespace WpfStudentChat.Models.Messages;

public class FriendDecreasedMessage(User friend)
{
    public User Friend { get; } = friend;
}
