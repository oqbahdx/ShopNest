using MediatR;
using ShopNest.Application.Features.DTOs;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Orders.Queries.GetOrderSummary;

public sealed class GetOrderSummaryQueryHandler
    : IRequestHandler<GetOrderSummaryQuery, Result<OrderSummaryDto>>
{
    private readonly IAppDbContext _db;

    public GetOrderSummaryQueryHandler(IAppDbContext db) => _db = db;

    public async Task<Result<OrderSummaryDto>> Handle(
        GetOrderSummaryQuery _, CancellationToken ct)
    {
        // Single query — group by status in the DB
        var counts = await _db.Orders
            .AsNoTracking()
            .GroupBy(o => o.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.Status, g => g.Count, ct);

        int Get(OrderStatus s) => counts.GetValueOrDefault(s, 0);

        var dto = new OrderSummaryDto(
            Pending: Get(OrderStatus.Pending),
            Confirmed: Get(OrderStatus.Confirmed),
            Processing: Get(OrderStatus.Processing),
            Shipped: Get(OrderStatus.Shipped),
            Delivered: Get(OrderStatus.Delivered),
            Cancelled: Get(OrderStatus.Cancelled),
            ReturnRequested: Get(OrderStatus.ReturnRequested)
        );

        return Result<OrderSummaryDto>.Success(dto);
    }
}