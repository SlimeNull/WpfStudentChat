namespace StudentChat.Models
{
    public class Group
    {
        public int Id { get; set; }
        public int AvatarId { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int OwnerId { get; set; }
    }
}
