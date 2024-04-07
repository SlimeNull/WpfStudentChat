namespace WpfStudentChat.Server.Models.Database
{
    public class PrivateMessageImageAttachment
    {
        public int Id { get; set; }

        public int MessageId { get; set; }
        public int AttachmentId { get; set; }

        public PrivateMessage Message { get; set; } = null!;
        public ImageBinary Attachment { get; set; } = null!;
    }
}
