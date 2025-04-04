namespace Tandia.Identity.Infrastructure.Models;

public sealed class UserCredentialsEntity(Guid id, string email, string passwordHash, string salt)
{
    public Guid Id { get; } = id;

    public string Email { get; } = email;

    public string PasswordHash { get; } = passwordHash;

    public string Salt { get; } = salt;
}
