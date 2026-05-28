namespace ShopNest.Application.Common.Interfaces;

/// <summary>
/// Transactional email abstraction. All methods are fire-and-log — never throw to callers.
/// </summary>
public interface IEmailService
{
    Task SendEmailConfirmationAsync(string toEmail, string userName,
        string confirmationLink, CancellationToken ct = default);

    Task SendPasswordResetAsync(string toEmail, string userName,
        string resetLink, CancellationToken ct = default);

    Task SendPasswordChangedNotificationAsync(string toEmail, string userName,
        CancellationToken ct = default);

    Task SendAccountLockedNotificationAsync(string toEmail, string userName,
        CancellationToken ct = default);
}
