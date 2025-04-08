using Tandia.Identity.Application.Enums;
using Tandia.Identity.Application.Models;
using Tandia.Identity.Application.Models.Responses;
using Tandia.Identity.Application.Services.Interfaces;
using Tandia.Identity.Infrastructure.Models;
using Tandia.Identity.Infrastructure.Repositories;

namespace Tandia.Identity.Application.Services;

public sealed class IdentityService(

    IRepository<UserEntity> userRepository,
    IRepository<UserCredentialsEntity> credentialsRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IPasswordService passwordService,
    ITokenProvider tokenProvider,
    TimeProvider timeProvider)
    : IIdentityService
{
    public async Task<UserStatus> RegisterUserAsync(string email, string password)
    {
        if (await credentialsRepository.GetByEmailAsync(email) != null)
        {
            return UserStatus.LoginFailed;
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

        return UserStatus.Registered;
    }

    public async Task<LoginResponse> LoginUserAsync(string email, string password)
    {
        var userCredentials = await credentialsRepository.GetByEmailAsync(email);

        if (userCredentials == null || !passwordService.VerifyPassword(password, userCredentials.PasswordHash, userCredentials.Salt))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
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

        return new LoginResponse(accessToken, refreshToken.Token);
    }

    public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
    {
        var refreshTokenEntity = await refreshTokenRepository.GetTokenAsync(refreshToken);

        if (refreshTokenEntity == null || refreshTokenEntity.ExpiryDate < timeProvider.GetUtcNow())
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        // Генерация нового access токена
        var newAccessToken = tokenProvider.GenerateAccessToken(refreshTokenEntity.UserId);

        // Генерация нового refresh токена
        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = tokenProvider.GenerateRefreshToken(),
            ExpiryDate = timeProvider.GetUtcNow().AddDays(7),
            UserId = refreshTokenEntity.UserId,
        };

        // Сохранение нового refresh токена в базу данных
        await refreshTokenRepository.AddAsync(new RefreshTokenEntity(
            newRefreshToken.Id,
            newRefreshToken.UserId,
            newRefreshToken.Token,
            newRefreshToken.ExpiryDate));

        return new LoginResponse(newAccessToken, newRefreshToken.Token);
    }
}
