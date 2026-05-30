using MediatR;

namespace ShopNest.Application.Features.Notifications.Commands.MarkAllRead;

public sealed record MarkAllNotificationsReadCommand : IRequest<Result>;

public sealed class MarkAllNotificationsReadCommandHandler
    : IRequestHandler<MarkAllNotificationsReadCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public MarkAllNotificationsReadCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        MarkAllNotificationsReadCommand _, CancellationToken ct)
    {
        // Single bulk UPDATE — avoid loading every entity into memory
        await _db.Notifications
            .Where(n =>
                n.UserId == _currentUser.UserId &&
                !n.IsRead)
            .ExecuteUpdateAsync(
                s => s.SetProperty(n => n.IsRead, true), ct);

        return Result.Success();
    }
}