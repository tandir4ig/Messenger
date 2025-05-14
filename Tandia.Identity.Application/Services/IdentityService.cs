using CSharpFunctionalExtensions;
using Tandia.Identity.Infrastructure.Models;
using Tandia.Identity.Infrastructure.Repositories;
using Tandia.Identity.WebApi.Models;
using Tandia.Identity.WebApi.Models.Responses;
using Tandia.Identity.WebApi.Services.Interfaces;

namespace Tandia.Identity.WebApi.Services;

public sealed class IdentityService(

    IUserRepository userRepository,
    IUserCredentialsRepository credentialsRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordService passwordService,
    ITokenProvider tokenProvider,
    TimeProvider timeProvider)
    : IIdentityService
{
    public async Task<Result> RegisterUserAsync(string email, string password)
    {
        if (await credentialsRepository.GetByEmailAsync(email) != null)
        {
            return Result.Failure("Пользователь с таким Email уже существует");
        }

        var userId = Guid.NewGuid();
        var registrationDate = timeProvider.GetUtcNow();

        // Хеширование пароля
        var (hashedPassword, salt) = passwordService.HashPassword(password);

        // Создание пользователя
        var userEntity = new UserEntity(
            userId,
            registrationDate);

        // Создание учетных данных
        var credentialsEntity = new UserCredentialsEntity(
            userId,
            email,
            hashedPassword,
            salt);

        // Сохранение в базе данных
        await userRepository.AddAsync(userEntity);
        await credentialsRepository.AddAsync(credentialsEntity);

        return Result.Success();
    }

    public async Task<Result<LoginResponse>> LoginUserAsync(string email, string password)
    {
        var userCredentials = await credentialsRepository.GetByEmailAsync(email);

        if (userCredentials == null)
        {
            return Result.Failure<LoginResponse>("Пользователь с таким email не найден.");
        }

        if (!passwordService.VerifyPassword(password, userCredentials.PasswordHash, userCredentials.Salt))
        {
            return Result.Failure<LoginResponse>("Неверный пароль.");
        }

        var accessToken = tokenProvider.GenerateAccessToken(userCredentials.Id);
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = tokenProvider.GenerateRefreshToken(),
            ExpiryDate = timeProvider.GetUtcNow().AddDays(7),
            UserId = userCredentials.Id,
        };

        await refreshTokenRepository.AddAsync(new RefreshTokenEntity(
            refreshToken.Id,
            refreshToken.UserId,
            refreshToken.Token,
            refreshToken.ExpiryDate));

        return Result.Success(new LoginResponse(accessToken, refreshToken.Token));
    }

    public async Task<Result<LoginResponse>> RefreshTokenAsync(string refreshToken)
    {
        var refreshTokenEntity = await refreshTokenRepository.GetTokenAsync(refreshToken);

        if (refreshTokenEntity == null || refreshTokenEntity.ExpiryDate < timeProvider.GetUtcNow())
        {
            return Result.Failure<LoginResponse>("Refresh-токен недействителен или истёк.");
        }

        var newAccessToken = tokenProvider.GenerateAccessToken(refreshTokenEntity.UserId);
        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = tokenProvider.GenerateRefreshToken(),
            ExpiryDate = timeProvider.GetUtcNow().AddDays(7),
            UserId = refreshTokenEntity.UserId,
        };

        await refreshTokenRepository.DeleteAsync(refreshToken);

        await refreshTokenRepository.AddAsync(new RefreshTokenEntity(
            newRefreshToken.Id,
            newRefreshToken.UserId,
            newRefreshToken.Token,
            newRefreshToken.ExpiryDate));

        return Result.Success(new LoginResponse(newAccessToken, newRefreshToken.Token));
    }
}
