using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Common.Identity;

namespace ShopNest.Application.Features.Auth.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler
    : IRequestHandler<ForgotPasswordCommand, Result>
{
    private readonly UserManager<AppUser>                     _userManager;
    private readonly IEmailService                            _emailService;
    private readonly ILogger<ForgotPasswordCommandHandler>    _logger;

    public ForgotPasswordCommandHandler(UserManager<AppUser> userManager,
        IEmailService emailService, ILogger<ForgotPasswordCommandHandler> logger)
    {
        _userManager  = userManager;
        _emailService = emailService;
        _logger       = logger;
    }

    public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        // Always return success — never reveal if email is registered
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || !user.EmailConfirmed)
            return Result.Success();

        var token   = await _userManager.GeneratePasswordResetTokenAsync(user);
        var payload = $"RESET_TOKEN::{Uri.EscapeDataString(user.Email!)}::{Uri.EscapeDataString(token)}";

        await _emailService.SendPasswordResetAsync(
            user.Email!, user.FullName, payload, ct);

        _logger.LogInformation("Password reset email dispatched for user {UserId}", user.Id);
        return Result.Success();
    }
}
