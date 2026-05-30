using MediatR;
using ShopNest.Domain.DomainEvents;
using ShopNest.Domain.Entities;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Notifications.EventHandlers;

/// <summary>
/// Triggered by CancelOrderCommand and by Phase 4 charge.refunded webhook.
/// </summary>
public sealed class OrderCancelledEventHandler
    : INotificationHandler<OrderCancelledDomainEvent>
{
    private readonly IAppDbContext  _db;
    private readonly IEmailService  _email;

    public OrderCancelledEventHandler(IAppDbContext db, IEmailService email)
    {
        _db    = db;
        _email = email;
    }

    public async Task Handle(
        OrderCancelledDomainEvent ev, CancellationToken ct)
    {
        var order = await _db.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == ev.OrderId, ct);

        if (order is null) return;

        var notification = Notification.Create(
            userId:  ev.UserId,
            type:    NotificationType.OrderCancelled,
            title:   "Order Cancelled",
            message: $"Order #{order.OrderNumber} has been cancelled. " +
                     $"Any applicable refund will be processed within 3–5 business days."
        );
        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync(ct);

        var user = await _db.Users.FindAsync(
            new object[] { ev.UserId }, ct);

        if (user?.Email is not null)
        {
            await _email.SendCancellationNotificationAsync(
                user.Email, order.OrderNumber);
        }
    }
}