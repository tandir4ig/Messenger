using CSharpFunctionalExtensions;
using Tandia.Identity.Application.Models.Responses;

namespace Tandia.Identity.Application.Services.Interfaces;

public interface IIdentityService
{
    Task<Result> RegisterUserAsync(string email, string password);

    Task<Result<LoginResponse>> LoginUserAsync(string email, string password);

    Task<Result<LoginResponse>> RefreshTokenAsync(string refreshToken);
}
