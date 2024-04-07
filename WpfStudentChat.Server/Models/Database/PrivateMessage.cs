﻿namespace WpfStudentChat.Server.Models.Database
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
    }
}