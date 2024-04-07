using System.Security.Cryptography;
using System.Text;

namespace StudentChat.Server.Utilities
{
    public static class HashUtils
    {
        public static string SHA256Text(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            byte[] hash = SHA256.HashData(bytes);
            string hashText = Convert.ToHexString(hash);

            return hashText;
        }
    }
}
