namespace StudentChat.Models
{
    public class PrivateMessage
    {
        public int Id { get; set; }

        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;

        public DateTimeOffset SentTime { get; set; }

        public List<Attachment>? ImageAttachments { get; set; }
        public List<Attachment>? FileAttachments { get; set; }
    }
}
