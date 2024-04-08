namespace StudentChat.Models
{
    public record class Attachment
    {
        public string Name { get; set; } = string.Empty;
        public string AttachmentHash { get; set; } = string.Empty;
    }
}
