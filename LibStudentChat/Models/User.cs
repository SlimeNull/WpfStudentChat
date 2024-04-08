using System.Text.Json.Serialization;

namespace StudentChat.Models;

public record class User : IIdentifiable
{
    public int Id { get; set; }
    public string AvatarHash { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;

    [JsonIgnore]
    public string Name => Nickname;
}
