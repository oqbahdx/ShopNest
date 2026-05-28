using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ShopNest.Application.Common.Interfaces;

namespace ShopNest.Infrastructure.Services;

/// <summary>
/// Reads the current user identity from the ASP.NET Core HTTP context.
/// Registered as Scoped — one instance per HTTP request.
/// </summary>
public sealed class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _http;

    public CurrentUserService(IHttpContextAccessor http) => _http = http;

    private ClaimsPrincipal? Principal => _http.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var value = Principal?.FindFirstValue("uid")
                     ?? Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email =>
        Principal?.FindFirstValue(ClaimTypes.Email)
        ?? Principal?.FindFirstValue(JwtClaimNames.Email);

    public bool IsAuthenticated =>
        Principal?.Identity?.IsAuthenticated is true;

    public string? IpAddress =>
        _http.HttpContext?.Connection.RemoteIpAddress?.ToString();

    // Local helper — avoids a dependency on System.IdentityModel.Tokens.Jwt
    private static class JwtClaimNames
    {
        public const string Email = "email";
    }
}
