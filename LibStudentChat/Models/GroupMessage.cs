namespace StudentChat.Models;

public class GroupMessage
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public int GroupId { get; set; }
    public string Content { get; set; } = string.Empty;

    public DateTimeOffset SentTime { get; set; }
}
