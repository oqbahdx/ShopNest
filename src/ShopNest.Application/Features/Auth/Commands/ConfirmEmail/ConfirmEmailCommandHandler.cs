using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ShopNest.Application.Common.Identity;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Auth.Commands.ConfirmEmail;

public sealed class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ILogger<ConfirmEmailCommandHandler> _logger;

    public ConfirmEmailCommandHandler(UserManager<AppUser> userManager,
        ILogger<ConfirmEmailCommandHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken ct)
    {
        if (!Guid.TryParse(request.UserId, out _))
            return Result.Failure("Invalid confirmation link.", ErrorCodes.InvalidToken);

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user is null)
            return Result.Failure("Invalid confirmation link.", ErrorCodes.InvalidToken);

        if (user.EmailConfirmed)
            return Result.Success(); // idempotent

        var result = await _userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            _logger.LogWarning("Email confirmation failed for {UserId}: {Errors}",
                request.UserId, errors);
            return Result.Failure(
                "Invalid or expired confirmation token.", ErrorCodes.InvalidToken);
        }

        _logger.LogInformation("Email confirmed for user {UserId}", request.UserId);
        return Result.Success();
    }
}