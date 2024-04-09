namespace StudentChat.Models
{
    public record class FriendRequest
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }

        public string SenderName { get; set; } = string.Empty;
        public string ReceiverName { get; set; } = string.Empty;

        public string? Message { get; set; }
        public string? RejectReason { get; set; }

        public bool IsDone { get; set; }

        public DateTimeOffset SentTime { get; set; }
    }
}
