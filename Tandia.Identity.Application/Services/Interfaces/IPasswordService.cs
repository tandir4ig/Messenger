namespace Tandia.Identity.WebApi.Services.Interfaces;

public interface IPasswordService
{
    (string HashedPassword, string Salt) HashPassword(string password);

    bool VerifyPassword(string password, string hashedPassword, string salt);
}
