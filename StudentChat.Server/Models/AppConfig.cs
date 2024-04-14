namespace StudentChat.Server.Models;

public class AppConfig
{
    public string JwtSecret { get; set; } = string.Empty;
    public string JwtIssuer { get; set; } = string.Empty;
    public string JwtAudience { get; set; } = string.Empty;

    public string ManagerUserName { get; set; } = string.Empty;
    public string ManagerPassword { get; set; } = string.Empty;
}
