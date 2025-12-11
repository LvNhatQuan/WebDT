using System.Security.Cryptography;
using System.Text;

namespace WebD_T.Helper
{
    public static class PasswordHelper
    {
        // Tạo Salt (RandomKey)
        public static string GenerateRandomKey(int length = 5)
        {
            var pattern = @"fksjifjeijkgjioewmmvnTUYOMBGYHCGJKLHJHJK*&^%%&%^%&!";
            var rd = new Random();
            var sb = new StringBuilder();

            for (int i = 0; i < length; i++)
                sb.Append(pattern[rd.Next(pattern.Length)]);

            return sb.ToString();
        }

        // Hash SHA256(password + salt) - chuẩn hướng dẫn
        public static string ToSHA256Hash(this string password, string? saltKey)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + saltKey));
            return Convert.ToBase64String(bytes);
        }
    }
}
