using System.ComponentModel.DataAnnotations;

namespace StudentChat.Server.Models.Database;

public class FileBinary
{
    [Key]
    public string Hash { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;

    public byte[]? Data { get; set; }
}
