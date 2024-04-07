namespace LibStudentChat.Models;

public class User
{
    public int Id { get; set; }
    public int AvatarId { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;
}
