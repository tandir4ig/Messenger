namespace Tandia.Identity.WebApi.Models.Responses;

public sealed class LoginResponse(string accessToken, string refreshToken)
{
    public string AccessToken { get; } = accessToken;

    public string RefreshToken { get; } = refreshToken;
}
