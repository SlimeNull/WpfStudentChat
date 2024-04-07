namespace StudentChat.Server.Models.Database
{
    public class GroupMessageImageAttachment
    {
        public int Id { get; set; }

        public int MessageId { get; set; }
        public int AttachmentId { get; set; }

        public GroupMessage Message { get; set; } = null!;
        public ImageBinary Attachment { get; set; } = null!;
    }
}
