namespace Tandia.Identity.WebApi.Models;

public sealed class RefreshToken
{
    public required Guid Id { get; init; }

    public required Guid UserId { get; init; }

    public required string Token { get; init; }

    public required DateTimeOffset ExpiryDate { get; init; }
}
