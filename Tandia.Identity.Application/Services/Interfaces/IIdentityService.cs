using Tandia.Identity.Application.Enums;
using Tandia.Identity.Application.Models.Responses;

namespace Tandia.Identity.Application.Services.Interfaces;

public interface IIdentityService
{
    Task<UserStatus> RegisterUserAsync(string email, string password);

    Task<LoginResponse> LoginUserAsync(string email, string password);
}
