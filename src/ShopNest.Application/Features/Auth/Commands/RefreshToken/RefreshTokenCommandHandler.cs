using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShopNest.Application.Common.Identity;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Common.Settings;
using ShopNest.Application.Features.Auth.Commands.Login;
using ShopNest.Domain.Entities;

namespace ShopNest.Application.Features.Auth.Commands.RefreshToken;

/// <summary>
/// Rotates refresh tokens using a one-time-use model.
/// Detects token reuse attacks by revoking the entire token family when
/// a previously-revoked token is presented again.
/// </summary>
public sealed class RefreshTokenCommandHandler
    : IRequestHandler<RefreshTokenCommand, Result<AuthTokenPair>>
{
    private readonly IAppDbContext                        _db;
    private readonly UserManager<AppUser>                _userManager;
    private readonly IJwtService                         _jwtService;
    private readonly JwtSettings                         _jwt;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        IAppDbContext                        db,
        UserManager<AppUser>                userManager,
        IJwtService                         jwtService,
        IOptions<JwtSettings>               jwtSettings,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _db          = db;
        _userManager = userManager;
        _jwtService  = jwtService;
        _jwt         = jwtSettings.Value;
        _logger      = logger;
    }

    public async Task<Result<AuthTokenPair>> Handle(
        RefreshTokenCommand request, CancellationToken ct)
    {
        var hash  = _jwtService.HashRefreshToken(request.RawRefreshToken);
        var token = await _db.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == hash, ct);

        // ── Token not found ───────────────────────────────────────────────────
        if (token is null)
        {
            _logger.LogWarning("Refresh attempt with unknown token from {IP}", request.IpAddress);
            return Result<AuthTokenPair>.Failure("Invalid refresh token.", ErrorCodes.InvalidToken);
        }

        // ── Already revoked ── possible token-theft attack ────────────────────
        if (token.IsRevoked)
        {
            _logger.LogCritical(
                "SECURITY ALERT: Revoked token reuse for user {UserId} from {IP}. " +
                "Revoking all active tokens for this user.", token.UserId, request.IpAddress);

            await _db.RefreshTokens
                .Where(t => t.UserId == token.UserId && !t.IsRevoked)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(t => t.IsRevoked,   true)
                    .SetProperty(t => t.RevokedAt,   DateTime.UtcNow)
                    .SetProperty(t => t.RevokedByIp, request.IpAddress), ct);

            await _db.SaveChangesAsync(ct);

            return Result<AuthTokenPair>.Failure(
                "Security violation detected. All sessions have been revoked. Please sign in again.",
                ErrorCodes.SuspectedTokenTheft);
        }

        // ── Expired ───────────────────────────────────────────────────────────
        if (token.IsExpired)
        {
            _logger.LogInformation("Expired refresh token for user {UserId}", token.UserId);
            return Result<AuthTokenPair>.Failure(
                "Refresh token has expired. Please sign in again.", ErrorCodes.TokenExpired);
        }

        // ── Account state ─────────────────────────────────────────────────────
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == token.UserId, ct);

        if (user is null)
            return Result<AuthTokenPair>.Failure("User not found.", ErrorCodes.NotFound);

        if (!user.IsActive)
            return Result<AuthTokenPair>.Failure(
                "Account is deactivated.", ErrorCodes.AccountDeactivated);

        // ── Rotate: revoke old, create new ────────────────────────────────────
        var newRaw  = _jwtService.GenerateRefreshToken();
        var newHash = _jwtService.HashRefreshToken(newRaw);

        token.Revoke(request.IpAddress, newHash);   // marks old token as replaced

        await _db.RefreshTokens.AddAsync(new Domain.Entities.RefreshToken
        {
            UserId      = token.UserId,
            TokenHash   = newHash,
            ExpiresAt   = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpiryDays),
            CreatedByIp = request.IpAddress
        }, ct);

        await _db.SaveChangesAsync(ct);

        var roles  = await _userManager.GetRolesAsync(user);
        var access = _jwtService.GenerateAccessToken(user, roles);
        var expiry = DateTime.UtcNow.AddMinutes(_jwt.AccessTokenExpiryMinutes);

        _logger.LogInformation("Token rotated for user {UserId}", token.UserId);

        return Result<AuthTokenPair>.Success(new AuthTokenPair(access, expiry, newRaw));
    }
}
