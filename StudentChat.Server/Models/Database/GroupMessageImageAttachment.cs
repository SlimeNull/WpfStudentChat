namespace StudentChat.Server.Models.Database;

public class GroupMessageImageAttachment
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public int MessageId { get; set; }
    public string AttachmentHash { get; set; } = string.Empty;

    public GroupMessage Message { get; set; } = null!;
    public ImageBinary Attachment { get; set; } = null!;
}
