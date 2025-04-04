using Tandia.Identity.Application.Enums;

namespace Tandia.Identity.Application.Services.Interfaces;

public interface IIdentityService
{
    Task<UserStatus> RegisterUserAsync(string email, string password);

    Task<UserStatus> LoginUserAsync(string email, string password);
}
