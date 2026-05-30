using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShopNest.Application.Common.Identity;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Common.Settings;
using ShopNest.Domain.Entities;

namespace ShopNest.Application.Features.Auth.Commands.Login;

/// <summary>
/// Authenticates a user enforcing all account-state rules, then issues a
/// JWT access token and a persisted, hashed refresh token.
/// </summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthTokenPair>>
{
    private readonly UserManager<AppUser>         _userManager;
    private readonly IJwtService                  _jwtService;
    private readonly IEmailService                _emailService;
    private readonly IAppDbContext                _db;
    private readonly JwtSettings                  _jwt;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        UserManager<AppUser>            userManager,
        IJwtService                     jwtService,
        IEmailService                   emailService,
        IAppDbContext                   db,
        IOptions<JwtSettings>           jwtSettings,
        ILogger<LoginCommandHandler>    logger)
    {
        _userManager  = userManager;
        _jwtService   = jwtService;
        _emailService = emailService;
        _db           = db;
        _jwt          = jwtSettings.Value;
        _logger       = logger;
    }

    public async Task<Result<AuthTokenPair>> Handle(LoginCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Login attempt from {IP} for {Email}", request.IpAddress, request.Email);

        // 1 ── User lookup (same timing path for unknown email → prevents enumeration)
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
        {
            _logger.LogWarning("Login failed — unknown email: {Email}", request.Email);
            return Fail(ErrorCodes.InvalidCredentials, "Invalid email or password.");
        }

        // 2 ── Deactivated account
        if (!user.IsActive)
            return Fail(ErrorCodes.AccountDeactivated, "This account has been deactivated.");

        // 3 ── Email confirmation required
        if (!user.EmailConfirmed)
            return Fail(ErrorCodes.EmailNotConfirmed,
                "Please confirm your email address before signing in.");

        // 4 ── Already locked out
        if (await _userManager.IsLockedOutAsync(user))
        {
            _logger.LogWarning("Login rejected — locked account {UserId}", user.Id);
            return Fail(ErrorCodes.AccountLocked,
                $"Account is locked. Please try again after {user.LockoutEnd:HH:mm} UTC.");
        }

        // 5 ── Password verification
        if (!await _userManager.CheckPasswordAsync(user, request.Password))
        {
            await _userManager.AccessFailedAsync(user);

            // Check if this failure triggered lockout
            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning("Account {UserId} locked after repeated failures", user.Id);
                await _emailService.SendAccountLockedNotificationAsync(
                    user.Email!, user.FullName, ct);
                return Fail(ErrorCodes.AccountLocked,
                    "Too many failed attempts. Account is temporarily locked.");
            }

            return Fail(ErrorCodes.InvalidCredentials, "Invalid email or password.");
        }

        // 6 ── Reset failure counter on successful auth
        await _userManager.ResetAccessFailedCountAsync(user);

        // 7 ── Generate token pair
        var roles       = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtService.GenerateAccessToken(user, roles);
        var rawRefresh  = _jwtService.GenerateRefreshToken();
        var hashRefresh = _jwtService.HashRefreshToken(rawRefresh);

        // 8 ── Persist hashed refresh token (NEVER store raw token)
        await _db.RefreshTokens.AddAsync(new Domain.Entities.RefreshToken
        {
            UserId      = user.Id,
            TokenHash   = hashRefresh,
            ExpiresAt   = DateTime.UtcNow.AddDays(_jwt.RefreshTokenExpiryDays),
            CreatedByIp = request.IpAddress
        }, ct);

        // 9 ── Housekeeping: remove fully expired tokens for this user
        await _db.RefreshTokens
            .Where(t => t.UserId == user.Id && t.ExpiresAt < DateTime.UtcNow)
            .ExecuteDeleteAsync(ct);

        await _db.SaveChangesAsync(ct);

        _logger.LogInformation("User {UserId} authenticated successfully", user.Id);

        return Result<AuthTokenPair>.Success(new AuthTokenPair(
            accessToken,
            DateTime.UtcNow.AddMinutes(_jwt.AccessTokenExpiryMinutes),
            rawRefresh));
    }

    private static Result<AuthTokenPair> Fail(string code, string msg)
        => Result<AuthTokenPair>.Failure(msg, code);
}
