namespace StudentChat.Models;

public interface IIdentifiable
{
    public int Id { get; }
    public string Name { get; }
    public string AvatarHash { get; }
}