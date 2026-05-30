using MediatR;
using ShopNest.Domain.DomainEvents;
using ShopNest.Domain.Entities;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Notifications.EventHandlers;

/// <summary>
/// Triggered by Phase 4 webhook handler after payment_intent.succeeded.
/// </summary>
public sealed class OrderConfirmedEventHandler
    : INotificationHandler<OrderConfirmedDomainEvent>
{
    private readonly IAppDbContext _db;
    private readonly IEmailService _email;

    public OrderConfirmedEventHandler(IAppDbContext db, IEmailService email)
    {
        _db = db;
        _email = email;
    }

    public async Task Handle(
        OrderConfirmedDomainEvent ev, CancellationToken ct)
    {
        // Load the order to get the order number and total for the receipt
        var order = await _db.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == ev.OrderId, ct);

        if (order is null) return;

        var notification = Notification.Create(
            userId: ev.UserId,
            type: NotificationType.OrderConfirmed,
            title: "Payment Confirmed",
            message: $"Payment for order #{order.OrderNumber} " +
                     $"({order.TotalAmount:C}) was successful. " +
                     $"Your order is being prepared."
        );
        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync(ct);

        var user = await _db.Users.FindAsync(
            new object[] { ev.UserId }, ct);

        if (user?.Email is not null)
        {
            await _email.SendPaymentReceiptAsync(
                user.Email, order.OrderNumber, order.TotalAmount);
        }
    }
}