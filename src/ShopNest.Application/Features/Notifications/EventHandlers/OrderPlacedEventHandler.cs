using MediatR;
using ShopNest.Domain.DomainEvents;
using ShopNest.Domain.Entities;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Notifications.EventHandlers;

public sealed class OrderPlacedEventHandler
    : INotificationHandler<OrderPlacedDomainEvent>
{
    private readonly IAppDbContext _db;
    private readonly IEmailService _email;

    public OrderPlacedEventHandler(IAppDbContext db, IEmailService email)
    {
        _db = db;
        _email = email;
    }

    public async Task Handle(
        OrderPlacedDomainEvent ev, CancellationToken ct)
    {
        // 1. Create in-app notification
        var notification = Notification.Create(
            userId: ev.UserId,
            type: NotificationType.OrderPlaced,
            title: "Order Placed",
            message: $"Your order #{ev.OrderNumber} has been received. " +
                     $"Total: {ev.TotalAmount:C}. Awaiting payment."
        );
        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync(ct);

        // 2. Send transactional email (best-effort — does not throw)
        var user = await _db.Users.FindAsync(
            new object[] { ev.UserId }, ct);

        if (user?.Email is not null)
        {
            await _email.SendOrderConfirmationAsync(
                user.Email, ev.OrderNumber, ev.TotalAmount);
        }
    }
}