namespace Tandia.Identity.WebApi.Models;

internal sealed class UserCredentials
{
    public Guid Id { get; init; }

    public required string Email { get; init; }

    public required string PasswordHash { get; init; }

    public required string Salt { get; init; }
}
