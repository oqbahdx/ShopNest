using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopNest.Application.Features.Cart.DTOs;
using ShopNest.Application.Features.Cart.Mappers;

namespace ShopNest.Application.Features.Cart.Commands.RemoveCoupon;

public sealed class RemoveCouponCommandHandler
    : IRequestHandler<RemoveCouponCommand, Result<CartDto>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public RemoveCouponCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<CartDto>> Handle(
        RemoveCouponCommand _, CancellationToken ct)
    {
        var cart = await _db.Carts
            .Include(c => c.Items).ThenInclude(i => i.Product)
            .ThenInclude(p => p.Images)
            .Include(c => c.Coupon)
            .FirstOrDefaultAsync(
                c => c.UserId == _currentUser.UserId, ct);

        if (cart is null)
            return Result<CartDto>.Failure(
                "Cart not found.", ErrorCodes.NOT_FOUND);

        cart.RemoveCoupon(); // clears CouponId
        cart.Recalculate();
        await _db.SaveChangesAsync(ct);

        return Result<CartDto>.Success(CartMapper.ToDto(cart));
    }
}