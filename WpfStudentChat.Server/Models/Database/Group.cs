using CommonModels = LibStudentChat.Models;

namespace WpfStudentChat.Server.Models.Database
{
    public class Group
    {
        public int Id { get; set; }
        public int AvatarId { get; set; }

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
                AvatarId = group.AvatarId,
                Name = group.Name,
                Description = group.Description,
                OwnerId = group.OwnerId,
            };
        }
    }
}
