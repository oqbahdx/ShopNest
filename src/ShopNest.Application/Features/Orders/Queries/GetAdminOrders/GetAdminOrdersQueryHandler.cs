using MediatR;
using ShopNest.Application.Features.DTOs;

namespace ShopNest.Application.Features.Orders.Queries.GetAdminOrders;

public sealed class GetAdminOrdersQueryHandler
    : IRequestHandler<GetAdminOrdersQuery, Result<PagedResult<AdminOrderDto>>>
{
    private readonly IAppDbContext _db;

    public GetAdminOrdersQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<PagedResult<AdminOrderDto>>> Handle(
        GetAdminOrdersQuery qry, CancellationToken ct)
    {
        var q = _db.Orders
            .AsNoTracking()
            .Include(o => o.ShippingAddress)
            .Include(o => o.Items)
            .AsQueryable();

        if (qry.Status.HasValue)
            q = q.Where(o => o.Status == qry.Status.Value);

        if (qry.UserId.HasValue)
            q = q.Where(o => o.UserId == qry.UserId.Value);

        if (qry.From.HasValue)
            q = q.Where(o => o.CreatedAt >= qry.From.Value);

        if (qry.To.HasValue)
            q = q.Where(o => o.CreatedAt <= qry.To.Value);

        if (qry.MinAmount.HasValue)
            q = q.Where(o => o.TotalAmount >= qry.MinAmount.Value);

        if (qry.MaxAmount.HasValue)
            q = q.Where(o => o.TotalAmount <= qry.MaxAmount.Value);

        var total = await q.CountAsync(ct);

        q = (qry.SortBy.ToLower(), qry.SortOrder.ToLower()) switch
        {
            ("total", "asc") => q.OrderBy(o => o.TotalAmount),
            ("total", _) => q.OrderByDescending(o => o.TotalAmount),
            ("status", "asc") => q.OrderBy(o => o.Status),
            ("status", _) => q.OrderByDescending(o => o.Status),
            ("createdat", "asc") => q.OrderBy(o => o.CreatedAt),
            _ => q.OrderByDescending(o => o.CreatedAt)
        };

        var orders = await q
            .Skip((qry.Page - 1) * qry.PageSize)
            .Take(qry.PageSize)
            .ToListAsync(ct);

        var items = orders.Select(o => new AdminOrderDto(
            Id: o.Id,
            OrderNumber: o.OrderNumber,
            Status: o.Status,
            UserId: o.UserId,
            CustomerEmail: string.Empty,
            ItemCount: o.Items.Count,
            SubTotal: o.SubTotal,
            DiscountAmount: o.DiscountAmount,
            TotalAmount: o.TotalAmount,
            CouponCode: o.CouponCode,
            TrackingNumber: o.TrackingNumber,
            CreatedAt: o.CreatedAt
        )).ToList();

        return Result<PagedResult<AdminOrderDto>>.Success(
            PagedResult<AdminOrderDto>.Create(
                items, qry.Page, qry.PageSize, total));
    }
}