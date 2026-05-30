using MediatR;
using ShopNest.Domain.DomainEvents;
using ShopNest.Domain.Entities;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Notifications.EventHandlers;

public sealed class OrderShippedEventHandler
    : INotificationHandler<OrderShippedDomainEvent>
{
    private readonly IAppDbContext _db;
    private readonly IEmailService _email;

    public OrderShippedEventHandler(IAppDbContext db, IEmailService email)
    {
        _db = db;
        _email = email;
    }

    public async Task Handle(
        OrderShippedDomainEvent ev, CancellationToken ct)
    {
        var notification = Notification.Create(
            userId: ev.UserId,
            type: NotificationType.OrderShipped,
            title: "Order Shipped",
            message: $"Order #{ev.OrderNumber} is on its way! " +
                     $"Tracking: {ev.TrackingNumber}"
        );
        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync(ct);

        var user = await _db.Users.FindAsync(
            new object[] { ev.UserId }, ct);

        if (user?.Email is not null)
        {
            await _email.SendShippingNotificationAsync(
                user.Email, ev.OrderNumber, ev.TrackingNumber);
        }
    }
}