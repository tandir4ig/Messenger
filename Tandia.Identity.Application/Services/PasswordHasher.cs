using System.Security.Cryptography;
using System.Text;

namespace Tandia.Identity.Application.Services;

internal static class PasswordHasher
{
    public static (string HashedPassword, string Salt) HashPassword(string password)
    {
        // Генерация соли
        var salt = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        var hash = HashPasswordWithSalt(Encoding.UTF8.GetBytes(password), salt);

        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    public static bool VerifyPassword(string password, string hashedPassword, string salt)
    {
        var temp = Convert.ToBase64String(HashPasswordWithSalt(Encoding.UTF8.GetBytes(password), Encoding.UTF8.GetBytes(salt)));
        return temp == hashedPassword;
    }

    private static byte[] HashPasswordWithSalt(byte[] passwordBytes, byte[] salt)
    {
        using var sha256 = SHA256.Create();
        var saltedPassword = new byte[passwordBytes.Length + salt.Length];
        Array.Copy(passwordBytes, saltedPassword, passwordBytes.Length);
        Array.Copy(salt, 0, saltedPassword, passwordBytes.Length, salt.Length);
        return sha256.ComputeHash(saltedPassword);
    }
}
