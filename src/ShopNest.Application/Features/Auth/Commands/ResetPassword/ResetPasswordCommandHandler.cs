using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopNest.Application.Common.Identity;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Auth.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly UserManager<AppUser>                    _userManager;
    private readonly IEmailService                           _emailService;
    private readonly IApplicationDbContext                   _db;
    private readonly ILogger<ResetPasswordCommandHandler>    _logger;

    public ResetPasswordCommandHandler(
        UserManager<AppUser> userManager,
        IEmailService        emailService,
        IApplicationDbContext db,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _userManager  = userManager;
        _emailService = emailService;
        _db           = db;
        _logger       = logger;
    }

    public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            // Generic message — never confirm email existence
            return Result.Failure("Invalid password reset request.", ErrorCodes.InvalidToken);

        var identityResult = await _userManager.ResetPasswordAsync(
            user, request.Token, request.NewPassword);

        if (!identityResult.Succeeded)
        {
            var errors = string.Join("; ", identityResult.Errors.Select(e => e.Description));
            _logger.LogWarning("Password reset failed for {UserId}: {Errors}", user.Id, errors);
            return Result.Failure("Invalid or expired reset token.", ErrorCodes.InvalidToken);
        }

        // Force sign-out everywhere: revoke all active refresh tokens
        await _db.RefreshTokens
            .Where(t => t.UserId == user.Id && !t.IsRevoked)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.IsRevoked, true)
                .SetProperty(t => t.RevokedAt, DateTime.UtcNow), ct);

        await _emailService.SendPasswordChangedNotificationAsync(
            user.Email!, user.FullName, ct);

        _logger.LogInformation("Password reset completed for user {UserId}", user.Id);
        return Result.Success();
    }
}
