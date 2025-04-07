using Tandia.Identity.Application.Enums;

namespace Tandia.Identity.Application.Models.Responses;

public sealed class LoginResponse(string accessToken, string refreshToken)
{
    public string AccessToken { get; } = accessToken;

    public string RefreshToken { get; } = refreshToken;

    public UserStatus Status { get; set; }
}
