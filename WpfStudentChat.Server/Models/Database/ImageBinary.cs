using System.ComponentModel.DataAnnotations;

namespace WpfStudentChat.Server.Models.Database
{
    public class ImageBinary
    {
        [Key]
        public string Hash { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;

        public byte[]? Data { get; set; }
    }
}
