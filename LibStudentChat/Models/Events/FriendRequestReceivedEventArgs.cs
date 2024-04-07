namespace StudentChat.Models.Events
{
    public class FriendRequestReceivedEventArgs(FriendRequest request) : EventArgs
    {
        public FriendRequest Request { get; } = request;
    }
}
