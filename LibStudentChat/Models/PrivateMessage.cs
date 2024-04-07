namespace LibStudentChat.Models
{
    public class PrivateMessage
    {
        public int Id { get; set; }

        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;

        public DateTimeOffset SentTime { get; set; }
    }
}
