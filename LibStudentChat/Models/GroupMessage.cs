namespace StudentChat.Models;

public record class GroupMessage : Message
{
    public int GroupId { get; set; }
}
