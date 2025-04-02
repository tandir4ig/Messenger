namespace Tandia.Identity.Infrastructure.Models;

public class UserEntity(Guid id, DateTimeOffset registrationDate)
{
    public Guid Id { get; } = id;

    public DateTimeOffset RegistrationDate { get; } = registrationDate;
}
