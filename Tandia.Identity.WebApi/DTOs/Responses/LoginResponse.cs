namespace Tandia.Identity.WebApi.DTOs.Responses;

public sealed record LoginResponse(string AccessToken, string RefreshToken);
