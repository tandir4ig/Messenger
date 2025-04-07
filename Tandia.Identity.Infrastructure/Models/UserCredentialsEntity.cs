namespace Tandia.Identity.Infrastructure.Models;

public sealed class UserCredentialsEntity(Guid id, string email, string passwordHash, string salt, string? refreshToken)
{
    public Guid Id { get; } = id;

    public string Email { get; } = email;

    public string PasswordHash { get; } = passwordHash;

    public string Salt { get; } = salt;

    public string? RefreshToken { get; } = refreshToken;
}
