using CommonModels = StudentChat.Models;

namespace StudentChat.Server.Models.Database
{
    public class Group
    {
        public int Id { get; set; }
        public string AvatarHash { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int OwnerId { get; set; }


        public User Owner { get; set; } = null!;
        public List<User> Members { get; } = new();
        public List<GroupMessage> Messages { get; } = new();
        public List<GroupRequest> Requests { get; } = new();


        public static explicit operator CommonModels.Group(Group group)
        {
            return new CommonModels.Group()
            {
                Id = group.Id,
                AvatarHash = group.AvatarHash,
                Name = group.Name,
                Description = group.Description,
                OwnerId = group.OwnerId,
            };
        }
    }
}
