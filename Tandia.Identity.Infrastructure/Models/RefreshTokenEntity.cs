namespace Tandia.Identity.Infrastructure.Models;

public sealed class RefreshTokenEntity(Guid id, Guid userId, string token, DateTimeOffset expiryDate, bool isValid)
{
    public Guid Id { get; } = id;

    public Guid UserId { get; } = userId;

    public string Token { get; } = token;

    public DateTimeOffset ExpiryDate { get; } = expiryDate;

    public bool IsValid { get; } = isValid;
}
