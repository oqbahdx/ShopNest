using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ShopNest.Application.Common.Identity;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Auth.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
{
    private readonly UserManager<AppUser>                       _userManager;
    private readonly IEmailService                              _emailService;
    private readonly IApplicationDbContext                               _db;
    private readonly ILogger<ChangePasswordCommandHandler>      _logger;

    public ChangePasswordCommandHandler(
        UserManager<AppUser> userManager,
        IEmailService        emailService,
        IApplicationDbContext         db,
        ILogger<ChangePasswordCommandHandler> logger)
    {
        _userManager  = userManager;
        _emailService = emailService;
        _db           = db;
        _logger       = logger;
    }

    public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var user = await _userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return Result.Failure("User not found.", ErrorCodes.NotFound);

        var result = await _userManager.ChangePasswordAsync(
            user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Change password failed for {UserId}: {Errors}",
                request.UserId, errors);
            return Result.Failure(errors, ErrorCodes.PasswordMismatch);
        }

        // Revoke all refresh tokens — force re-login on all devices
        await _db.RefreshTokens
            .Where(t => t.UserId == request.UserId && !t.IsRevoked)
            .ExecuteUpdateAsync(s => s
                .SetProperty(t => t.IsRevoked, true)
                .SetProperty(t => t.RevokedAt, DateTime.UtcNow), ct);

        await _emailService.SendPasswordChangedNotificationAsync(
            user.Email!, user.FullName, ct);

        _logger.LogInformation("Password changed for user {UserId}", request.UserId);
        return Result.Success();
    }
}
