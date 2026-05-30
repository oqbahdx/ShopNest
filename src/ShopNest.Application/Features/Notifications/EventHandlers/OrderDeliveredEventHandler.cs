using MediatR;
using ShopNest.Domain.DomainEvents;
using ShopNest.Domain.Entities;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Notifications.EventHandlers;

public sealed class OrderDeliveredEventHandler
    : INotificationHandler<OrderDeliveredDomainEvent>
{
    private readonly IAppDbContext _db;
    private readonly IEmailService _email;

    public OrderDeliveredEventHandler(IAppDbContext db, IEmailService email)
    {
        _db = db;
        _email = email;
    }

    public async Task Handle(
        OrderDeliveredDomainEvent ev, CancellationToken ct)
    {
        var notification = Notification.Create(
            userId: ev.UserId,
            type: NotificationType.OrderDelivered,
            title: "Order Delivered",
            message: $"Order #{ev.OrderNumber} has been delivered. " +
                     $"Enjoying your purchase? Leave a review!"
        );
        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync(ct);

        var user = await _db.Users.FindAsync(
            new object[] { ev.UserId }, ct);

        if (user?.Email is not null)
        {
            await _email.SendDeliveryConfirmationAsync(
                user.Email, ev.OrderNumber);
        }
    }
}