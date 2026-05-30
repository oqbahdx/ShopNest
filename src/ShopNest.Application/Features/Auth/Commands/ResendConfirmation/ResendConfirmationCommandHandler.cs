using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ShopNest.Application.Common.Identity;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Auth.Commands.ResendConfirmation;

public sealed class ResendConfirmationCommandHandler
    : IRequestHandler<ResendConfirmationCommand, Result>
{
    private readonly UserManager<AppUser>                         _userManager;
    private readonly IEmailService                                _emailService;
    private readonly ILogger<ResendConfirmationCommandHandler>    _logger;

    public ResendConfirmationCommandHandler(UserManager<AppUser> userManager,
        IEmailService emailService,
        ILogger<ResendConfirmationCommandHandler> logger)
    {
        _userManager  = userManager;
        _emailService = emailService;
        _logger       = logger;
    }

    public async Task<Result> Handle(ResendConfirmationCommand request, CancellationToken ct)
    {
        // Always return success — prevents email enumeration
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null || user.EmailConfirmed)
            return Result.Success();

        var token   = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var payload = $"CONFIRMATION_TOKEN::{user.Id}::{Uri.EscapeDataString(token)}";

        await _emailService.SendEmailConfirmationAsync(
            user.Email!, user.FullName, payload, ct);

        _logger.LogInformation("Confirmation email resent for user {UserId}", user.Id);
        return Result.Success();
    }
}
