using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ShopNest.Application.Common.Identity;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Auth.Commands.Register;

/// <summary>
/// Creates a new customer account and dispatches an email confirmation link.
/// Does NOT auto-login — the user must confirm their email first.
/// </summary>
public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<string>>
{
    private readonly UserManager<AppUser>            _userManager;
    private readonly IEmailService                   _emailService;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        UserManager<AppUser>            userManager,
        IEmailService                   emailService,
        ILogger<RegisterCommandHandler> logger)
    {
        _userManager  = userManager;
        _emailService = emailService;
        _logger       = logger;
    }

    public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Registration attempt for {Email}", request.Email);

        // 1 ── Duplicate email guard
        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
        {
            _logger.LogWarning("Registration rejected — email already taken: {Email}", request.Email);
            return Result<string>.Failure("Email already registered.", ErrorCodes.EmailAlreadyRegistered);
        }

        // 2 ── Build user entity
        var user = new AppUser
        {
            FirstName = request.FirstName.Trim(),
            LastName  = request.LastName.Trim(),
            Email     = request.Email.Trim().ToLowerInvariant(),
            UserName  = request.Email.Trim().ToLowerInvariant(),
        };

        // 3 ── Persist (Identity handles PBKDF2 password hashing internally)
        var createResult = await _userManager.CreateAsync(user, request.Password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
            _logger.LogWarning("Identity creation failed for {Email}: {Errors}", request.Email, errors);
            return Result<string>.Failure(errors, ErrorCodes.IdentityError);
        }

        // 4 ── Assign default role
        await _userManager.AddToRoleAsync(user, "Customer");

        // 5 ── Generate confirmation token and send email
        //      The raw token placeholder is resolved into a full URL inside EmailService.
        var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var tokenPayload = $"CONFIRMATION_TOKEN::{user.Id}::{Uri.EscapeDataString(confirmToken)}";

        await _emailService.SendEmailConfirmationAsync(
            user.Email!, user.FullName, tokenPayload, ct);

        _logger.LogInformation("User {UserId} registered successfully", user.Id);

        return Result<string>.Success(
            "Registration successful. Please check your email to confirm your account.");
    }
}
