namespace Tandia.Identity.Application.Models;

internal sealed class User
{
    public required Guid Id { get; init; }

    public required DateTimeOffset RegistrationDate { get; init; }
}
