namespace WpfStudentChat.Server.Models.Database
{
    public class GroupMessage
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int GroupId { get; set; }
        public string Content { get; set; } = string.Empty;

        public DateTimeOffset SentTime { get; set; }

        public User Sender { get; set; } = null!;
        public Group Group { get; set; } = null!;

        public List<GroupMessageImageAttachment> ImageAttachments { get; } = new();
        public List<GroupMessageFileAttachment> FileAttachments { get; } = new();
    }
}
