using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Infrastructure.Settings;

namespace ShopNest.Infrastructure.Services;

public sealed class EmailService : IEmailService
{
	private readonly EmailSettings _settings;

	private readonly ILogger<EmailService> _logger;

	public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger)
	{
		_settings = options.Value;
		_logger = logger;
	}

	public async Task SendEmailConfirmationAsync(string toEmail, string userName, string confirmationLink, CancellationToken ct = default(CancellationToken))
	{
		string link = ResolveLink(confirmationLink, "confirm-email", "userId", "token");
		await SendAsync(toEmail, "Confirm your ShopNest account", $"<div style=\"font-family:Arial,sans-serif;max-width:600px;margin:auto\">\n  <h2 style=\"color:#1E3A5F\">Welcome to ShopNest, {userName}!</h2>\n  <p>Please confirm your email address to activate your account.</p>\n  <a href=\"{link}\"\n     style=\"display:inline-block;padding:12px 28px;background:#1E3A5F;\n            color:#fff;border-radius:6px;text-decoration:none;font-weight:bold\">\n    Confirm Email Address\n  </a>\n  <p style=\"color:#888;font-size:13px;margin-top:24px\">\n    Link expires in 24 hours. If you did not register, you can safely ignore this email.\n  </p>\n</div>", ct);
	}

	public async Task SendPasswordResetAsync(string toEmail, string userName, string resetLink, CancellationToken ct = default(CancellationToken))
	{
		string link = ResolveLink(resetLink, "reset-password", "email", "token");
		await SendAsync(toEmail, "Reset your ShopNest password", $"<div style=\"font-family:Arial,sans-serif;max-width:600px;margin:auto\">\n  <h2 style=\"color:#E07B39\">Password Reset Request</h2>\n  <p>Hi {userName},</p>\n  <p>We received a request to reset your ShopNest password.</p>\n  <a href=\"{link}\"\n     style=\"display:inline-block;padding:12px 28px;background:#E07B39;\n            color:#fff;border-radius:6px;text-decoration:none;font-weight:bold\">\n    Reset Password\n  </a>\n  <p style=\"color:#888;font-size:13px;margin-top:24px\">\n    This link expires in 1 hour. If you did not request a reset,\n    please <a href=\"{_settings.FrontendBaseUrl}/support\">contact support</a>.\n  </p>\n</div>", ct);
	}

	public async Task SendPasswordChangedNotificationAsync(string toEmail, string userName, CancellationToken ct = default(CancellationToken))
	{
		await SendAsync(toEmail, "Your ShopNest password was changed", $"<div style=\"font-family:Arial,sans-serif;max-width:600px;margin:auto\">\n  <h2 style=\"color:#1E3A5F\">Password Changed</h2>\n  <p>Hi {userName},</p>\n  <p>Your ShopNest account password was successfully updated.</p>\n  <p>If you did not make this change, please\n     <a href=\"{_settings.FrontendBaseUrl}/forgot-password\">reset your password</a>\n     immediately and contact our support team.\n  </p>\n</div>", ct);
	}

	public async Task SendAccountLockedNotificationAsync(string toEmail, string userName, CancellationToken ct = default(CancellationToken))
	{
		await SendAsync(toEmail, "ShopNest account temporarily locked", $"<div style=\"font-family:Arial,sans-serif;max-width:600px;margin:auto\">\n  <h2 style=\"color:#C0392B\">Account Temporarily Locked</h2>\n  <p>Hi {userName},</p>\n  <p>Your account has been locked due to multiple failed login attempts.</p>\n  <p>You may\n     <a href=\"{_settings.FrontendBaseUrl}/forgot-password\">reset your password</a>\n     or try again in 15 minutes.\n  </p>\n</div>", ct);
	}

	public async Task SendOrderConfirmationAsync(string toEmail, string orderNumber, decimal totalAmount, CancellationToken ct = default(CancellationToken))
	{
		await SendAsync(toEmail, $"Order {orderNumber} received", $"<div style=\"font-family:Arial,sans-serif;max-width:600px;margin:auto\">\n  <h2 style=\"color:#1E3A5F\">Order Received</h2>\n  <p>Your order <strong>#{orderNumber}</strong> has been received.</p>\n  <p>Total: <strong>{totalAmount:C}</strong></p>\n</div>", ct);
	}

	public async Task SendPaymentReceiptAsync(string toEmail, string orderNumber, decimal totalAmount, CancellationToken ct = default(CancellationToken))
	{
		await SendAsync(toEmail, $"Payment receipt for {orderNumber}", $"<div style=\"font-family:Arial,sans-serif;max-width:600px;margin:auto\">\n  <h2 style=\"color:#1E3A5F\">Payment Confirmed</h2>\n  <p>Payment for order <strong>#{orderNumber}</strong> was successful.</p>\n  <p>Amount paid: <strong>{totalAmount:C}</strong></p>\n</div>", ct);
	}

	public async Task SendShippingNotificationAsync(string toEmail, string orderNumber, string trackingNumber, CancellationToken ct = default(CancellationToken))
	{
		await SendAsync(toEmail, $"Order {orderNumber} shipped", $"<div style=\"font-family:Arial,sans-serif;max-width:600px;margin:auto\">\n  <h2 style=\"color:#1E3A5F\">Order Shipped</h2>\n  <p>Your order <strong>#{orderNumber}</strong> is on its way.</p>\n  <p>Tracking number: <strong>{trackingNumber}</strong></p>\n</div>", ct);
	}

	public async Task SendDeliveryConfirmationAsync(string toEmail, string orderNumber, CancellationToken ct = default(CancellationToken))
	{
		await SendAsync(toEmail, $"Order {orderNumber} delivered", $"<div style=\"font-family:Arial,sans-serif;max-width:600px;margin:auto\">\n  <h2 style=\"color:#1E3A5F\">Order Delivered</h2>\n  <p>Your order <strong>#{orderNumber}</strong> has been delivered.</p>\n</div>", ct);
	}

	public async Task SendCancellationNotificationAsync(string toEmail, string orderNumber, CancellationToken ct = default(CancellationToken))
	{
		await SendAsync(toEmail, $"Order {orderNumber} cancelled", $"<div style=\"font-family:Arial,sans-serif;max-width:600px;margin:auto\">\n  <h2 style=\"color:#C0392B\">Order Cancelled</h2>\n  <p>Your order <strong>#{orderNumber}</strong> has been cancelled.</p>\n  <p>Any applicable refund will be processed according to the payment provider timeline.</p>\n</div>", ct);
	}

	private async Task SendAsync(string toEmail, string subject, string html, CancellationToken ct)
	{
		try
		{
			SendGridClient client = new SendGridClient(_settings.SendGridApiKey);
			EmailAddress from = new EmailAddress(_settings.FromEmail, _settings.FromName);
			EmailAddress to = new EmailAddress(toEmail);
			SendGridMessage message = MailHelper.CreateSingleEmail(from, to, subject, string.Empty, html);
			Response response = await client.SendEmailAsync(message, ct);
			if (!response.IsSuccessStatusCode)
			{
				_logger.LogWarning("SendGrid returned {Status} sending to {Email} (subject: {Subject})", (int)response.StatusCode, toEmail, subject);
			}
		}
		catch (Exception ex)
		{
			Exception ex2 = ex;
			_logger.LogError(ex2, "Failed to send email to {Email} (subject: {Subject})", toEmail, subject);
		}
	}

	private string ResolveLink(string raw, string path, string param1, string param2)
	{
		if (!raw.Contains("::"))
		{
			return raw;
		}
		string[] array = raw.Split("::", 3);
		return (array.Length < 3) ? raw : $"{_settings.FrontendBaseUrl}/{path}?{param1}={array[1]}&{param2}={array[2]}";
	}
}
