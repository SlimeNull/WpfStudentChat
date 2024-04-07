using System.ComponentModel.DataAnnotations;

namespace WpfStudentChat.Server.Models.Database
{
    public class FileBinary
    {
        [Key]
        public string Hash { get; set; } = string.Empty;

        public byte[]? Data { get; set; }
    }
}
