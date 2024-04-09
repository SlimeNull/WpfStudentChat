using StudentChat.Models;

namespace WpfStudentChat.Models.Messages
{
    public class FriendRequestReceivedMessage(FriendRequest request)
    {
        public FriendRequest Request { get; } = request;
    }
}
