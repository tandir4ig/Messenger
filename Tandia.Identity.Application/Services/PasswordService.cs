using System.Security.Cryptography;
using System.Text;
using Tandia.Identity.Application.Services.Interfaces;

namespace Tandia.Identity.Application.Services;

public sealed class PasswordService : IPasswordService
{
    public (string HashedPassword, string Salt) HashPassword(string password)
    {
        // Генерация соли
        var salt = new byte[16];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(salt);

        var hash = HashPasswordWithSalt(Encoding.UTF8.GetBytes(password), salt);

        return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
    }

    public bool VerifyPassword(string password, string hashedPassword, string salt)
    {
        var temp = Convert.ToBase64String(HashPasswordWithSalt(Encoding.UTF8.GetBytes(password), Convert.FromBase64String(salt)));
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
