namespace StudentChat.Models
{
    public record class FriendRequest
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }

        public string? Message { get; set; }
    }
}
