namespace WpfStudentChat.Server.Models.Database
{
    public class GroupMessageFileAttachment
    {
        public int Id { get; set; }

        public int MessageId { get; set; }
        public int AttachmentId { get; set; }

        public GroupMessage Message { get; set; } = null!;
        public FileBinary Attachment { get; set; } = null!;
    }
}
