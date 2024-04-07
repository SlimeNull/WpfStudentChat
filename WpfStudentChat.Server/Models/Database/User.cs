namespace WpfStudentChat.Server.Models.Database
{
    public class User
    {
        public int Id { get; set; }
        public int AvatarId { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;

        public string UserName { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;

        public List<Group> JoindGroups { get; } = new();
        public List<Group> OwnedGroups { get; } = new();
        public List<PrivateMessage> SentPrivateMessages { get; } = new();
        public List<PrivateMessage> ReceivedPrivateMessages { get; } = new();

        public List<GroupMessage> SentGroupMessages { get; } = new();
    }
}
