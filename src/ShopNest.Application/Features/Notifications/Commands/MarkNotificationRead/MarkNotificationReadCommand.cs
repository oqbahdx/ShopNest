using MediatR;

namespace ShopNest.Application.Features.Notifications.Commands.MarkNotificationRead;

public sealed record MarkNotificationReadCommand(Guid NotificationId)
    : IRequest<Result>;

public sealed class MarkNotificationReadCommandHandler
    : IRequestHandler<MarkNotificationReadCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public MarkNotificationReadCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        MarkNotificationReadCommand cmd, CancellationToken ct)
    {
        var notification = await _db.Notifications
            .FirstOrDefaultAsync(n => n.Id == cmd.NotificationId, ct);

        if (notification is null)
            return Result.Failure(
                "Notification not found.", ErrorCodes.NOT_FOUND);

        if (notification.UserId != _currentUser.UserId)
            return Result.Failure("Access denied.", ErrorCodes.FORBIDDEN);

        notification.MarkRead();
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}