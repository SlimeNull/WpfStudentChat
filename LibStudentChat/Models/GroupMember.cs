namespace StudentChat.Models;

public record class GroupMember
{
    public int Id { get; set; }

    public int GroupId { get; set; }
    public int UserId { get; set; }
}
