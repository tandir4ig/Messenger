namespace Tandia.Identity.Application.Services.Interfaces;

internal interface IPasswordService
{
    (string HashedPassword, string Salt) HashPassword(string password);

    bool VerifyPassword(string password, string hashedPassword, string salt);
}
