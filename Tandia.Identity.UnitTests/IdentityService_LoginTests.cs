using System.Net.Mail;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using MassTransit;
using Moq;
using Tandia.Identity.Contracts.Events;
using Tandia.Identity.Infrastructure.Models;
using Tandia.Identity.Infrastructure.Repositories;
using Tandia.Identity.WebApi.Models.Responses;
using Tandia.Identity.WebApi.Services;
using Tandia.Identity.WebApi.Services.Interfaces;

namespace Tandia.Identity.UnitTests;

public sealed class IdentityService_LoginTests
{
    private readonly IFixture _fixture =
        new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });

    private readonly Mock<IUserCredentialsRepository> _credRepo;
    private readonly Mock<IRefreshTokenRepository> _tokenRepo;
    private readonly Mock<IPasswordService> _passwordService;
    private readonly Mock<ITokenProvider> _tokenProvider;
    private readonly Mock<IPublishEndpoint> _publishEndpoint;
    private readonly Mock<TimeProvider> _timeProvider;

    private readonly IdentityService sut;

    private readonly DateTimeOffset _now = new(2024, 04, 01, 12, 0, 0, TimeSpan.Zero);

    public IdentityService_LoginTests()
    {
        _credRepo = _fixture.Freeze<Mock<IUserCredentialsRepository>>();
        _tokenRepo = _fixture.Freeze<Mock<IRefreshTokenRepository>>();
        _passwordService = _fixture.Freeze<Mock<IPasswordService>>();
        _tokenProvider = _fixture.Freeze<Mock<ITokenProvider>>();
        _publishEndpoint = _fixture.Freeze<Mock<IPublishEndpoint>>();
        _timeProvider = _fixture.Freeze<Mock<TimeProvider>>();

        _timeProvider.Setup(tp => tp.GetUtcNow()).Returns(_now);

        sut = _fixture.Create<IdentityService>();
    }

    [Fact(DisplayName = "возвращает пару токенов и сохраняет refresh-токен, если учётные данные верны")]
    public async Task ReturnTokens_And_PersistRefreshToken_When_Credentials_Are_Correct()
    {
        // Arrange
        var email = _fixture.Create<MailAddress>().Address;
        var password = _fixture.Create<string>();
        var userId = _fixture.Create<Guid>();

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

        // Act
        var result = await sut.LoginUserAsync(email, password);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new LoginResponse("access-jwt", "refresh-jwt"));

        _tokenRepo.Verify(
            r => r.AddAsync(It.IsAny<RefreshTokenEntity>()),
            Times.Once,
            "refresh-токен должен быть сохранён один раз");

        _publishEndpoint.Verify(
            p => p.Publish(
                It.Is<UserLoggedIn>(e =>
                    e.Email == email),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _publishEndpoint.VerifyNoOtherCalls();
    }

    [Fact(DisplayName = "логин: отдаёт Failure, если пользователь не найден (публикации нет)")]
    public async Task ReturnFailure_And_NoPublish_When_UserNotFound()
    {
        // Arrange
        var email = _fixture.Create<MailAddress>().Address;

        _credRepo.Setup(r => r.GetByEmailAsync(email))
                 .ReturnsAsync((UserCredentialsEntity?)null);

        // Act
        var result = await sut.LoginUserAsync(email, _fixture.Create<string>());

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Пользователь с таким email не найден.");

        _publishEndpoint.VerifyNoOtherCalls();
    }

    [Fact(DisplayName = "логин: Failure, если пароль неверный (публикации нет)")]
    public async Task ReturnFailure_When_PasswordIsWrong()
    {
        // Arrange
        var email = _fixture.Create<MailAddress>().Address;
        var creds = new UserCredentialsEntity(Guid.NewGuid(), email, "hash", "salt");

        _credRepo.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(creds);
        _passwordService.Setup(s => s.VerifyPassword("wrong", creds.PasswordHash, creds.Salt))
                        .Returns(value: false);

        // Act
        var result = await sut.LoginUserAsync(email, "wrong");

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Неверный пароль.");

        _publishEndpoint.VerifyNoOtherCalls();
    }
}
