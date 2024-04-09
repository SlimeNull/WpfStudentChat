namespace StudentChat.Models
{
    public record class GroupRequest
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int GroupId { get; set; }

        public string SenderName { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;

        public string? Message { get; set; }
        public string? RejectReason { get; set; }

        public bool IsDone { get; set; }

        public DateTimeOffset SentTime { get; set; }
    }
}
