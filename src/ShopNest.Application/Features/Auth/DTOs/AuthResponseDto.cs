namespace ShopNest.Application.Features.Auth.DTOs;

/// <summary>
/// Returned on login / refresh. Refresh token is NOT here — it lives in an HttpOnly cookie.
/// </summary>
public sealed record AuthResponseDto(
    string   AccessToken,
    DateTime AccessTokenExpiry,
    string   TokenType = "Bearer");
