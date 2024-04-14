namespace StudentChat.Server.Models.Database;

public class PrivateMessageFileAttachment
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int MessageId { get; set; }
    public string AttachmentHash { get; set; } = string.Empty;

    public PrivateMessage Message { get; set; } = null!;
    public FileBinary Attachment { get; set; } = null!;
}
