using CommonModels = StudentChat.Models;

namespace StudentChat.Server.Models.Database
{
    public class User
    {
        public int Id { get; set; }
        public string AvatarHash { get; set; } = string.Empty;

        public string Nickname { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public List<User> AddedFriends { get; } = new();
        public List<User> AcceptedFriends { get; } = new();

        public List<Group> JoindGroups { get; } = new();
        public List<Group> OwnedGroups { get; } = new();
        public List<PrivateMessage> SentPrivateMessages { get; } = new();
        public List<PrivateMessage> ReceivedPrivateMessages { get; } = new();

        public List<GroupMessage> SentGroupMessages { get; } = new();

        public List<FriendRequest> SentFriendRequests { get; } = new();
        public List<FriendRequest> ReceivedFriendRequests { get; } = new();
        public List<GroupRequest> SentGroupRequests { get; } = new();


        public static explicit operator CommonModels.User(User user)
        {
            return new CommonModels.User()
            {
                Id = user.Id,
                AvatarHash = user.AvatarHash,
                Nickname = user.Nickname,
                Bio = user.Bio,
                UserName = user.UserName,
            };
        }
    }
}
