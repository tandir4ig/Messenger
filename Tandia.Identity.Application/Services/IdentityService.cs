using Tandia.Identity.Application.Enums;
using Tandia.Identity.Application.Models.Responses;
using Tandia.Identity.Application.Services.Interfaces;
using Tandia.Identity.Infrastructure.Models;
using Tandia.Identity.Infrastructure.Repositories;

namespace Tandia.Identity.Application.Services;

public sealed class IdentityService(

    IRepository<UserEntity> userRepository,
    IRepository<UserCredentialsEntity> credentialsRepository,
    IPasswordService passwordService,
    ITokenService tokenService,
    TimeProvider timeProvider)
    : IIdentityService
{
    public async Task<UserStatus> RegisterUserAsync(string email, string password)
    {
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
            salt,
            refreshToken: null);

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

        var accessToken = tokenService.GenerateAccessToken(userCredentials.Id);
        var refreshToken = tokenService.GenerateRefreshToken();

        return new LoginResponse(accessToken, refreshToken);
    }
}
