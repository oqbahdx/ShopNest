using MediatR;

namespace ShopNest.Application.Features.Notifications.Queries.GetUnreadCount;

public sealed record GetUnreadCountQuery : IRequest<Result<int>>;

public sealed class GetUnreadCountQueryHandler
    : IRequestHandler<GetUnreadCountQuery, Result<int>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetUnreadCountQueryHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<int>> Handle(
        GetUnreadCountQuery _, CancellationToken ct)
    {
        if (_currentUser.UserId is not Guid userId)
            return Result<int>.Failure("Authentication required.", ErrorCodes.FORBIDDEN);

        var count = await _db.Notifications
            .AsNoTracking()
            .CountAsync(n => n.UserId == userId && !n.IsRead, ct);

        return Result<int>.Success(count);
    }
}
