using FluentAssertions;
using Moq;
using Tandia.Identity.Infrastructure.Models;
using Tandia.Identity.Infrastructure.Repositories;
using Tandia.Identity.WebApi.Models.Responses;
using Tandia.Identity.WebApi.Services;
using Tandia.Identity.WebApi.Services.Interfaces;

namespace Tandia.Identity.UnitTests;

public sealed class IdentityService_LoginTests
{
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IUserCredentialsRepository> _credRepo = new();
    private readonly Mock<IRefreshTokenRepository> _tokenRepo = new();
    private readonly Mock<IPasswordService> _passwordService = new();
    private readonly Mock<ITokenProvider> _tokenProvider = new();
    private readonly Mock<TimeProvider> _timeProvider = new();

    private readonly DateTimeOffset _now = new(2024, 04, 01, 12, 0, 0, TimeSpan.Zero);

    private IdentityService CreateSut()
    {
        _timeProvider.Setup(tp => tp.GetUtcNow()).Returns(_now);

        return new IdentityService(
            _userRepo.Object,
            _credRepo.Object,
            _tokenRepo.Object,
            _passwordService.Object,
            _tokenProvider.Object,
            _timeProvider.Object);
    }

    [Fact(DisplayName = "возвращает пару токенов и сохраняет refresh-токен, если учётные данные верны")]
    public async Task ReturnTokens_And_PersistRefreshToken_When_Credentials_Are_Correct()
    {
        // Arrange
        const string email = "john@doe.io";
        const string password = "P@ssw0rd!";
        var userId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

        var creds = new UserCredentialsEntity(
            userId,
            email,
            "hash",
            salt: "salt");

        _credRepo
            .Setup(r => r.GetByEmailAsync(email))
            .ReturnsAsync(creds);

        _passwordService
            .Setup(s => s.VerifyPassword(password, creds.PasswordHash, creds.Salt))
            .Returns(value: true);

        _tokenProvider
            .Setup(p => p.GenerateAccessToken(userId))
            .Returns("access-jwt");
        _tokenProvider
            .Setup(p => p.GenerateRefreshToken())
            .Returns("refresh-jwt");

        _tokenRepo
            .Setup(r => r.AddAsync(It.IsAny<RefreshTokenEntity>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        var sut = CreateSut();

        // Act
        var result = await sut.LoginUserAsync(email, password);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new LoginResponse("access-jwt", "refresh-jwt"));

        _tokenRepo.Verify(
            r => r.AddAsync(It.IsAny<RefreshTokenEntity>()),
            Times.Once,
            "refresh-токен должен быть сохранён один раз");
    }

    [Fact(DisplayName = "отдаёт Failure, если пользователь не найден")]
    public async Task ReturnFailure_When_UserNotFound()
    {
        // Arrange
        const string email = "absent@domain.io";
        _credRepo.Setup(r => r.GetByEmailAsync(email))
                 .ReturnsAsync((UserCredentialsEntity?)null);

        var sut = CreateSut();

        // Act
        var result = await sut.LoginUserAsync(email, "irrelevant");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Пользователь с таким email не найден.");
    }

    [Fact(DisplayName = "отдаёт Failure, если пароль неверный")]
    public async Task ReturnFailure_When_PasswordIsWrong()
    {
        // Arrange
        const string email = "john@doe.io";
        var creds = new UserCredentialsEntity(Guid.NewGuid(), email, "hash", "salt");

        _credRepo.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(creds);
        _passwordService.Setup(s => s.VerifyPassword("wrong", creds.PasswordHash, creds.Salt))
                        .Returns(value: false);

        var sut = CreateSut();

        // Act
        var result = await sut.LoginUserAsync(email, "wrong");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Неверный пароль.");
    }
}
