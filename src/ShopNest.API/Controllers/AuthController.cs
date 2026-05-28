using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Auth.Commands.ChangePassword;
using ShopNest.Application.Features.Auth.Commands.ConfirmEmail;
using ShopNest.Application.Features.Auth.Commands.ForgotPassword;
using ShopNest.Application.Features.Auth.Commands.Login;
using ShopNest.Application.Features.Auth.Commands.Logout;
using ShopNest.Application.Features.Auth.Commands.Register;
using ShopNest.Application.Features.Auth.Commands.ResendConfirmation;
using ShopNest.Application.Features.Auth.Commands.ResetPassword;
using ShopNest.Application.Features.Auth.Commands.RevokeAllTokens;
using ShopNest.Application.Features.Auth.DTOs;
using ShopNest.Application.Features.Auth.Queries.GetCurrentUser;
using ShopNest.Application.Common.Settings;

namespace ShopNest.API.Controllers;

/// <summary>
/// Authentication and account-management endpoints.
/// Responsibility: HTTP ↔ MediatR mapping, cookie management, HTTP status selection.
/// Zero business logic lives here.
/// </summary>
[ApiController]
[Route("api/v1/auth")]
[Produces("application/json")]
public sealed class AuthController : ControllerBase
{
    private const string CookieName = "refreshToken";
    private const string CookiePath = "/api/v1/auth";

    private readonly IMediator   _mediator;
    private readonly JwtSettings _jwt;

    public AuthController(IMediator mediator, IOptions<JwtSettings> jwt)
    {
        _mediator = mediator;
        _jwt      = jwt.Value;
    }

    // ─────────────────────────────────────────────────────────────────────────
    // POST /api/v1/auth/register
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Register a new customer account. Sends a confirmation email.</summary>
    /// <response code="200">Registration successful; confirmation email sent.</response>
    /// <response code="409">Email already registered.</response>
    /// <response code="422">Validation errors.</response>
    [HttpPost("register")]
    [ProducesResponseType(200)]
    [ProducesResponseType(409)]
    [ProducesResponseType(422)]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new RegisterCommand(req.FirstName, req.LastName,
                req.Email, req.Password, req.ConfirmPassword), ct);

        return MapResult(result);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // POST /api/v1/auth/login
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Authenticate with email and password.
    /// Returns a JWT access token in the body and a refresh token in an HttpOnly cookie.
    /// </summary>
    /// <response code="200">Authentication successful.</response>
    /// <response code="401">Invalid credentials, unconfirmed email, or deactivated account.</response>
    /// <response code="423">Account is temporarily locked.</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(Envelope<AuthResponseDto>), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(423)]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new LoginCommand(req.Email, req.Password, ClientIp()), ct);

        if (result.IsFailure)
            return MapError(result.ErrorCode!, result.Error!);

        SetRefreshCookie(result.Data!.RawRefreshToken);

        return Ok(new Envelope<AuthResponseDto>(
            true, "Login successful",
            new AuthResponseDto(
                result.Data.AccessToken,
                result.Data.AccessTokenExpiry)));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // POST /api/v1/auth/refresh-token
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>
    /// Rotate the refresh token. Reads the token from the HttpOnly cookie.
    /// Returns a new access token and sets a new refresh-token cookie.
    /// </summary>
    /// <response code="200">Token rotated.</response>
    /// <response code="401">Token invalid, expired, or security violation detected.</response>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(CancellationToken ct)
    {
        var raw = Request.Cookies[CookieName];
        if (string.IsNullOrWhiteSpace(raw))
            return Unauthorized(Envelope.Fail("Refresh token cookie is missing."));

        var result = await _mediator.Send(
            new Application.Features.Auth.Commands.RefreshToken.RefreshTokenCommand(
                raw, ClientIp()), ct);

        if (result.IsFailure)
        {
            ClearRefreshCookie();
            return MapError(result.ErrorCode!, result.Error!);
        }

        SetRefreshCookie(result.Data!.RawRefreshToken);

        return Ok(new Envelope<AuthResponseDto>(
            true, "Token refreshed",
            new AuthResponseDto(
                result.Data.AccessToken,
                result.Data.AccessTokenExpiry)));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // POST /api/v1/auth/logout
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Revoke the current refresh token and clear the cookie.</summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var raw = Request.Cookies[CookieName] ?? string.Empty;
        await _mediator.Send(new LogoutCommand(raw, ClientIp()), ct);
        ClearRefreshCookie();
        return Ok(Envelope.Ok("Logged out successfully."));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GET /api/v1/auth/confirm-email
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Confirm a user email address using the link from the confirmation email.</summary>
    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(
        [FromQuery] string userId,
        [FromQuery] string token,
        CancellationToken ct)
    {
        var result = await _mediator.Send(new ConfirmEmailCommand(userId, token), ct);
        return MapResult(result, "Email confirmed successfully.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // POST /api/v1/auth/resend-confirmation
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Resend the email confirmation link.</summary>
    [HttpPost("resend-confirmation")]
    public async Task<IActionResult> ResendConfirmation(
        [FromBody] ForgotPasswordRequest req, CancellationToken ct)
    {
        await _mediator.Send(new ResendConfirmationCommand(req.Email), ct);
        // Always 200 — prevents email enumeration
        return Ok(Envelope.Ok(
            "If that address is registered and unconfirmed, a new link has been sent."));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // POST /api/v1/auth/forgot-password
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Send a password-reset link to the given email.</summary>
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest req, CancellationToken ct)
    {
        await _mediator.Send(new ForgotPasswordCommand(req.Email), ct);
        // Always 200 — prevents email enumeration
        return Ok(Envelope.Ok(
            "If that address is registered, a password reset link has been sent."));
    }

    // ─────────────────────────────────────────────────────────────────────────
    // POST /api/v1/auth/reset-password
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Reset password using the token from the reset email.</summary>
    /// <response code="200">Password reset successful.</response>
    /// <response code="400">Invalid or expired token.</response>
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordRequest req, CancellationToken ct)
    {
        var result = await _mediator.Send(
            new ResetPasswordCommand(req.Email, req.Token,
                req.NewPassword, req.ConfirmNewPassword), ct);

        return MapResult(result, "Password has been reset. Please sign in.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // POST /api/v1/auth/change-password
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Change password for the currently authenticated user.</summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest req, CancellationToken ct)
    {
        var userId = CurrentUserId();
        if (userId is null) return Unauthorized();

        var result = await _mediator.Send(
            new ChangePasswordCommand(userId.Value,
                req.CurrentPassword, req.NewPassword, req.ConfirmNewPassword), ct);

        return MapResult(result, "Password changed successfully.");
    }

    // ─────────────────────────────────────────────────────────────────────────
    // GET /api/v1/auth/me
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Get the currently authenticated user profile.</summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(Envelope<CurrentUserDto>), 200)]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var userId = CurrentUserId();
        if (userId is null) return Unauthorized();

        var result = await _mediator.Send(new GetCurrentUserQuery(userId.Value), ct);
        return MapResult(result);
    }

    // ─────────────────────────────────────────────────────────────────────────
    // POST /api/v1/auth/revoke-all
    // ─────────────────────────────────────────────────────────────────────────

    /// <summary>Sign out of all devices by revoking every active session.</summary>
    [HttpPost("revoke-all")]
    [Authorize]
    public async Task<IActionResult> RevokeAll(CancellationToken ct)
    {
        var userId = CurrentUserId();
        if (userId is null) return Unauthorized();

        await _mediator.Send(new RevokeAllTokensCommand(userId.Value, ClientIp()), ct);
        ClearRefreshCookie();
        return Ok(Envelope.Ok("All sessions have been revoked."));
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private void SetRefreshCookie(string raw) =>
        Response.Cookies.Append(CookieName, raw, new CookieOptions
        {
            HttpOnly  = true,
            Secure    = true,
            SameSite  = SameSiteMode.Strict,
            Expires   = DateTimeOffset.UtcNow.AddDays(_jwt.RefreshTokenExpiryDays),
            Path      = CookiePath
        });

    private void ClearRefreshCookie() =>
        Response.Cookies.Delete(CookieName, new CookieOptions { Path = CookiePath });

    private Guid?   CurrentUserId() =>
        Guid.TryParse(User.FindFirst("uid")?.Value, out var id) ? id : null;

    private string? ClientIp() =>
        HttpContext.Connection.RemoteIpAddress?.ToString();

    // ── Result → HTTP mapping ─────────────────────────────────────────────────

    private IActionResult MapResult(Result result, string? successMsg = null) =>
        result.IsSuccess
            ? Ok(Envelope.Ok(successMsg ?? "Operation completed successfully."))
            : MapError(result.ErrorCode!, result.Error!);

    private IActionResult MapResult<T>(Result<T> result, string? successMsg = null) =>
        result.IsSuccess
            ? Ok(new Envelope<T>(true, successMsg ?? "Operation completed.", result.Data!))
            : MapError(result.ErrorCode!, result.Error!);

    private IActionResult MapError(string code, string message) => code switch
    {
        ErrorCodes.EmailAlreadyRegistered => Conflict(Envelope.Fail(message, code)),
        ErrorCodes.InvalidCredentials     => Unauthorized(Envelope.Fail(message, code)),
        ErrorCodes.AccountDeactivated     => Unauthorized(Envelope.Fail(message, code)),
        ErrorCodes.EmailNotConfirmed      => Unauthorized(Envelope.Fail(message, code)),
        ErrorCodes.AccountLocked          => StatusCode(423, Envelope.Fail(message, code)),
        ErrorCodes.InvalidToken           => BadRequest(Envelope.Fail(message, code)),
        ErrorCodes.TokenExpired           => Unauthorized(Envelope.Fail(message, code)),
        ErrorCodes.SuspectedTokenTheft    => Unauthorized(Envelope.Fail(message, code)),
        ErrorCodes.NotFound               => NotFound(Envelope.Fail(message, code)),
        ErrorCodes.PasswordMismatch       => BadRequest(Envelope.Fail(message, code)),
        ErrorCodes.ValidationError        => UnprocessableEntity(Envelope.Fail(message, code)),
        _                                 => BadRequest(Envelope.Fail(message, code))
    };
}

// ── API response envelope ─────────────────────────────────────────────────────

/// <summary>Standard JSON response wrapper used across all endpoints.</summary>
public sealed record Envelope<T>(bool Success, string Message, T Data);

public static class Envelope
{
    public static object Ok(string message)
        => new { success = true,  message };

    public static object Fail(string message, string? errorCode = null)
        => new { success = false, message, errorCode };
}
