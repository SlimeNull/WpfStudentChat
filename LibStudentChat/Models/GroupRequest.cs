namespace StudentChat.Models
{
    public record class GroupRequest
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int GroupId { get; set; }

        public string? Message { get; set; }
    }
}
