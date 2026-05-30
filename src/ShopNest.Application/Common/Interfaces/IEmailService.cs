using System.Threading;
using System.Threading.Tasks;

namespace ShopNest.Application.Common.Interfaces;

public interface IEmailService
{
	Task SendEmailConfirmationAsync(string toEmail, string userName, string confirmationLink, CancellationToken ct = default(CancellationToken));

	Task SendPasswordResetAsync(string toEmail, string userName, string resetLink, CancellationToken ct = default(CancellationToken));

	Task SendPasswordChangedNotificationAsync(string toEmail, string userName, CancellationToken ct = default(CancellationToken));

	Task SendAccountLockedNotificationAsync(string toEmail, string userName, CancellationToken ct = default(CancellationToken));

	Task SendOrderConfirmationAsync(string toEmail, string orderNumber, decimal totalAmount, CancellationToken ct = default(CancellationToken));

	Task SendPaymentReceiptAsync(string toEmail, string orderNumber, decimal totalAmount, CancellationToken ct = default(CancellationToken));

	Task SendShippingNotificationAsync(string toEmail, string orderNumber, string trackingNumber, CancellationToken ct = default(CancellationToken));

	Task SendDeliveryConfirmationAsync(string toEmail, string orderNumber, CancellationToken ct = default(CancellationToken));

	Task SendCancellationNotificationAsync(string toEmail, string orderNumber, CancellationToken ct = default(CancellationToken));
}
