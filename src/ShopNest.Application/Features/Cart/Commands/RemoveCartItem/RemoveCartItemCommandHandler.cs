using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopNest.Application.Features.Cart.DTOs;
using ShopNest.Application.Features.Cart.Mappers;

namespace ShopNest.Application.Features.Cart.Commands.RemoveCartItem;

public sealed class RemoveCartItemCommandHandler
    : IRequestHandler<RemoveCartItemCommand, Result<CartDto>>
{
    private readonly IAppDbContext       _db;
    private readonly ICurrentUserService _currentUser;

    public RemoveCartItemCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db          = db;
        _currentUser = currentUser;
    }

    public async Task<Result<CartDto>> Handle(
        RemoveCartItemCommand cmd, CancellationToken ct)
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

        var itemBelongsToCart = cart.Items.Any(i => i.Id == cmd.CartItemId);
        if (!itemBelongsToCart)
            return Result<CartDto>.Failure(
                "Cart item not found.", ErrorCodes.NOT_FOUND);

        cart.RemoveItem(cmd.CartItemId);
        cart.Recalculate();
        await _db.SaveChangesAsync(ct);

        return Result<CartDto>.Success(CartMapper.ToDto(cart));
    }
}