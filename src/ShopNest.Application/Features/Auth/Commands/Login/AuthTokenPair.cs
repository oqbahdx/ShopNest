namespace ShopNest.Application.Features.Auth.Commands.Login;

/// <summary>
/// Carries access + raw refresh token through the application boundary to the controller.
/// The raw refresh token is set as an HttpOnly cookie; it is never serialised to JSON.
/// </summary>
public sealed record AuthTokenPair(
    string   AccessToken,
    DateTime AccessTokenExpiry,
    string   RawRefreshToken);
