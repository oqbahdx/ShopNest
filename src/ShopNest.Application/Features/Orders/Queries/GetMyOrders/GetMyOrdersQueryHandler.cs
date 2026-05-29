using MediatR;
using ShopNest.Application.Features.DTOs;

namespace ShopNest.Application.Features.Orders.Queries.GetMyOrders;

public sealed class GetMyOrdersQueryHandler
    : IRequestHandler<GetMyOrdersQuery, Result<PagedResult<OrderListDto>>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetMyOrdersQueryHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<OrderListDto>>> Handle(
        GetMyOrdersQuery qry, CancellationToken ct)
    {
        var q = _db.Orders
            .AsNoTracking()
            .Where(o => o.UserId == _currentUser.UserId)
            .AsQueryable();

        if (qry.Status.HasValue)
            q = q.Where(o => o.Status == qry.Status.Value);

        var total = await q.CountAsync(ct);

        var orders = await q
            .OrderByDescending(o => o.CreatedAt)
            .Skip((qry.Page - 1) * qry.PageSize)
            .Take(qry.PageSize)
            .Select(o => new OrderListDto(
                o.Id,
                o.OrderNumber,
                o.Status,
                o.Items.Count,
                o.TotalAmount,
                o.TrackingNumber,
                o.CreatedAt))
            .ToListAsync(ct);

        return Result<PagedResult<OrderListDto>>.Success(
            PagedResult<OrderListDto>.Create(
                orders, qry.Page, qry.PageSize, total));
    }
}