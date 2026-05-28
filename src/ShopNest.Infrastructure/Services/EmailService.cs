using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Infrastructure.Settings;

namespace ShopNest.Infrastructure.Services;

/// <summary>
/// Sends transactional emails via SendGrid.
/// All methods swallow exceptions and log them — email failures are never
/// propagated to callers and never break business transactions.
/// </summary>
public sealed class EmailService : IEmailService
{
    private readonly EmailSettings         _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger)
    {
        _settings = options.Value;
        _logger   = logger;
    }

    public async Task SendEmailConfirmationAsync(string toEmail, string userName,
        string confirmationLink, CancellationToken ct = default)
    {
        var link = ResolveLink(confirmationLink, "confirm-email", "userId", "token");
        await SendAsync(toEmail, "Confirm your ShopNest account", $"""
            <div style="font-family:Arial,sans-serif;max-width:600px;margin:auto">
              <h2 style="color:#1E3A5F">Welcome to ShopNest, {userName}!</h2>
              <p>Please confirm your email address to activate your account.</p>
              <a href="{link}"
                 style="display:inline-block;padding:12px 28px;background:#1E3A5F;
                        color:#fff;border-radius:6px;text-decoration:none;font-weight:bold">
                Confirm Email Address
              </a>
              <p style="color:#888;font-size:13px;margin-top:24px">
                Link expires in 24 hours. If you did not register, you can safely ignore this email.
              </p>
            </div>
            """, ct);
    }

    public async Task SendPasswordResetAsync(string toEmail, string userName,
        string resetLink, CancellationToken ct = default)
    {
        var link = ResolveLink(resetLink, "reset-password", "email", "token");
        await SendAsync(toEmail, "Reset your ShopNest password", $"""
            <div style="font-family:Arial,sans-serif;max-width:600px;margin:auto">
              <h2 style="color:#E07B39">Password Reset Request</h2>
              <p>Hi {userName},</p>
              <p>We received a request to reset your ShopNest password.</p>
              <a href="{link}"
                 style="display:inline-block;padding:12px 28px;background:#E07B39;
                        color:#fff;border-radius:6px;text-decoration:none;font-weight:bold">
                Reset Password
              </a>
              <p style="color:#888;font-size:13px;margin-top:24px">
                This link expires in 1 hour. If you did not request a reset,
                please <a href="{_settings.FrontendBaseUrl}/support">contact support</a>.
              </p>
            </div>
            """, ct);
    }

    public async Task SendPasswordChangedNotificationAsync(string toEmail, string userName,
        CancellationToken ct = default)
    {
        await SendAsync(toEmail, "Your ShopNest password was changed", $"""
            <div style="font-family:Arial,sans-serif;max-width:600px;margin:auto">
              <h2 style="color:#1E3A5F">Password Changed</h2>
              <p>Hi {userName},</p>
              <p>Your ShopNest account password was successfully updated.</p>
              <p>If you did not make this change, please
                 <a href="{_settings.FrontendBaseUrl}/forgot-password">reset your password</a>
                 immediately and contact our support team.
              </p>
            </div>
            """, ct);
    }

    public async Task SendAccountLockedNotificationAsync(string toEmail, string userName,
        CancellationToken ct = default)
    {
        await SendAsync(toEmail, "ShopNest account temporarily locked", $"""
            <div style="font-family:Arial,sans-serif;max-width:600px;margin:auto">
              <h2 style="color:#C0392B">Account Temporarily Locked</h2>
              <p>Hi {userName},</p>
              <p>Your account has been locked due to multiple failed login attempts.</p>
              <p>You may
                 <a href="{_settings.FrontendBaseUrl}/forgot-password">reset your password</a>
                 or try again in 15 minutes.
              </p>
            </div>
            """, ct);
    }

    // ── Private helpers ───────────────────────────────────────────────────────

    private async Task SendAsync(string toEmail, string subject,
        string html, CancellationToken ct)
    {
        try
        {
            var client  = new SendGridClient(_settings.SendGridApiKey);
            var from    = new EmailAddress(_settings.FromEmail, _settings.FromName);
            var to      = new EmailAddress(toEmail);
            var message = MailHelper.CreateSingleEmail(from, to, subject, string.Empty, html);
            var response = await client.SendEmailAsync(message, ct);

            if (!response.IsSuccessStatusCode)
                _logger.LogWarning(
                    "SendGrid returned {Status} sending to {Email} (subject: {Subject})",
                    (int)response.StatusCode, toEmail, subject);
        }
        catch (Exception ex)
        {
            // Never propagate — email failure must not break business transactions
            _logger.LogError(ex,
                "Failed to send email to {Email} (subject: {Subject})", toEmail, subject);
        }
    }

    /// <summary>
    /// Converts a raw token placeholder string produced by handlers into a
    /// full frontend deep-link URL.
    /// Format: "CONFIRMATION_TOKEN::{userId}::{encodedToken}"
    ///         "RESET_TOKEN::{userId}::{encodedToken}"
    /// </summary>
    private string ResolveLink(string raw, string path, string param1, string param2)
    {
        if (!raw.Contains("::"))
            return raw; // already a URL

        var parts = raw.Split("::", 3);
        return parts.Length < 3
            ? raw
            : $"{_settings.FrontendBaseUrl}/{path}?{param1}={parts[1]}&{param2}={parts[2]}";
    }
}
