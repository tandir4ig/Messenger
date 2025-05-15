using CSharpFunctionalExtensions;
using Tandia.Identity.WebApi.Models.Responses;

namespace Tandia.Identity.WebApi.Services.Interfaces;

public interface IIdentityService
{
    Task<Result> RegisterUserAsync(string email, string password);

    Task<Result<LoginResponse>> LoginUserAsync(string email, string password);

    Task<Result<LoginResponse>> RefreshTokenAsync(string refreshToken);
}
