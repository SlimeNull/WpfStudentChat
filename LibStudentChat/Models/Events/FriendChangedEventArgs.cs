namespace StudentChat.Models.Events
{
    public class FriendChangedEventArgs(User friend) : EventArgs
    {
        public User Friend { get; } = friend;
    }
}
