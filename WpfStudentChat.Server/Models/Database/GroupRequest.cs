namespace WpfStudentChat.Server.Models.Database
{
    public class GroupRequest
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int GroupId { get; set; }

        public string Message { get; set; } = string.Empty;

        public User Sender { get; set; } = null!;
        public Group Group { get; set; } = null!;
    }
}
