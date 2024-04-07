namespace StudentChat.Server.Models.Database
{
    public class UserFriend
    {
        public int Id { get; set; }

        public int FromUserId { get; set; }
        public int ToUserId { get; set; }

        public User FromUser { get; set; } = null!;
        public User ToUser { get; set; } = null!;
    }
}
