namespace StudentChat.Server.Models.Database;

public class GroupMember
{
    public int Id { get; set; }

    public DateTimeOffset LastReadTime { get; set; }

    public int GroupId { get; set; }
    public int UserId { get; set; }

    public Group Group { get; set; } = null!;
    public User User { get; set; } = null!;
}
