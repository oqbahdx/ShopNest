using MediatR;
using ShopNest.Application.Features.Users.DTOs;

namespace ShopNest.Application.Features.Notifications.Queries.GetNotifications;

public sealed record GetNotificationsQuery(
    int Page = 1,
    int PageSize = 20,
    bool? IsRead = null
) : IRequest<Result<PagedResult<NotificationDto>>>;

public sealed class GetNotificationsQueryHandler
    : IRequestHandler<GetNotificationsQuery, Result<PagedResult<NotificationDto>>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetNotificationsQueryHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<NotificationDto>>> Handle(
        GetNotificationsQuery qry, CancellationToken ct)
    {
        var q = _db.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == _currentUser.UserId)
            .AsQueryable();

        if (qry.IsRead.HasValue)
            q = q.Where(n => n.IsRead == qry.IsRead.Value);

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(n => n.CreatedAt)
            .Skip((qry.Page - 1) * qry.PageSize)
            .Take(qry.PageSize)
            .Select(n => new NotificationDto(
                n.Id, n.Type, n.Title, n.Message,
                n.IsRead, n.CreatedAt))
            .ToListAsync(ct);

        return Result<PagedResult<NotificationDto>>.Success(
            PagedResult<NotificationDto>.Create(
                items, qry.Page, qry.PageSize, total));
    }
}