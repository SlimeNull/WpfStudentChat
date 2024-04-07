using CommonModels = StudentChat.Models;

namespace StudentChat.Server.Models.Database
{
    public class PrivateMessage
    {
        public int Id { get; set; }

        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;

        public DateTimeOffset SentTime { get; set; }

        public User Sender { get; set; } = null!;
        public User Receiver { get; set; } = null!;

        public List<PrivateMessageImageAttachment> ImageAttachments { get; } = new();
        public List<PrivateMessageFileAttachment> FileAttachments { get; } = new();


        public static explicit operator CommonModels.PrivateMessage(PrivateMessage message)
        {
            return new CommonModels.PrivateMessage()
            {
                Id = message.Id,
                SenderId = message.SenderId,
                ReceiverId = message.ReceiverId,
                Content = message.Content,
                SentTime = message.SentTime,
                ImageAttachments = message.ImageAttachments
                    ?.Select(attachment => new CommonModels.Attachment()
                    {
                        Name = attachment.Name,
                        AttachmentHash = attachment.AttachmentHash,
                    })
                    ?.ToList(),
                FileAttachments = message.FileAttachments
                    ?.Select(attachment => new CommonModels.Attachment()
                    {
                        Name = attachment.Name,
                        AttachmentHash = attachment.AttachmentHash,
                    })
                    ?.ToList()
            };
        }
    }
}
