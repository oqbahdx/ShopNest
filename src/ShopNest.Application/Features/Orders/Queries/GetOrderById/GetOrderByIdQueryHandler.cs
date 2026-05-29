using MediatR;
using ShopNest.Application.Features.DTOs;

namespace ShopNest.Application.Features.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdQueryHandler
    : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetOrderByIdQueryHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<OrderDto>> Handle(
        GetOrderByIdQuery qry, CancellationToken ct)
    {
        var order = await _db.Orders
            .AsNoTracking()
            .Include(o => o.ShippingAddress)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(o => o.Id == qry.Id, ct);

        if (order is null)
            return Result<OrderDto>.Failure(
                "Order not found.", ErrorCodes.NOT_FOUND);

        // Customers may only see their own orders
        if (!qry.IsAdmin && order.UserId != _currentUser.UserId)
            return Result<OrderDto>.Failure(
                "Access denied.", ErrorCodes.FORBIDDEN);

        return Result<OrderDto>.Success(OrderMapper.ToDto(order));
    }
}