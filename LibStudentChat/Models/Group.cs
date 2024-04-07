namespace StudentChat.Models;

public class Group : IIdentifiable
{
    public int Id { get; set; }
    public string AvatarHash { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public int OwnerId { get; set; }
}
