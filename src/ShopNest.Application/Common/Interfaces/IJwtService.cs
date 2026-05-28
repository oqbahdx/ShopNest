using ShopNest.Application.Common.Identity;

namespace ShopNest.Application.Common.Interfaces;

/// <summary>JWT creation abstraction — keeps Application layer free of JWT libraries.</summary>
public interface IJwtService
{
    /// <summary>Generates a signed JWT access token with user claims and roles.</summary>
    string GenerateAccessToken(AppUser user, IList<string> roles);

    /// <summary>Generates a cryptographically secure opaque refresh token (raw, unhashed).</summary>
    string GenerateRefreshToken();

    /// <summary>Returns SHA-256 hex hash of the raw token for safe DB storage.</summary>
    string HashRefreshToken(string rawToken);
}
