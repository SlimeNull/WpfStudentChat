using CommonModels = StudentChat.Models;

namespace StudentChat.Server.Models.Database
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

        public static explicit operator CommonModels.GroupMessage(GroupMessage message)
        {
            return new CommonModels.GroupMessage()
            {
                Id = message.Id,
                SenderId = message.SenderId,
                GroupId = message.GroupId,
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
