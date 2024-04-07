namespace WpfStudentChat.Server.Models.Database
{
    public class FriendRequest
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }

        public string Message { get; set; } = string.Empty;

        public User Sender { get; set; } = null!;
        public User Receiver { get; set; } = null!;
    }
}
