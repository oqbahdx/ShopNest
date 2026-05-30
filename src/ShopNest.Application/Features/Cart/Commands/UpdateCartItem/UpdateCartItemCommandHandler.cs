using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopNest.Application.Features.Cart.DTOs;
using ShopNest.Application.Features.Cart.Mappers;

namespace ShopNest.Application.Features.Cart.Commands.UpdateCartItem;

public sealed class UpdateCartItemCommandHandler
    : IRequestHandler<UpdateCartItemCommand, Result<CartDto>>
{
    private readonly IAppDbContext       _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateCartItemCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db          = db;
        _currentUser = currentUser;
    }

    public async Task<Result<CartDto>> Handle(
        UpdateCartItemCommand cmd, CancellationToken ct)
    {
        // 1. Load cart with items (ensures the item belongs to current user)
        var cart = await _db.Carts
            .Include(c => c.Items).ThenInclude(i => i.Product)
                .ThenInclude(p => p.Images)
            .Include(c => c.Coupon)
            .FirstOrDefaultAsync(
                c => c.UserId == _currentUser.UserId, ct);

        if (cart is null)
            return Result<CartDto>.Failure(
                "Cart not found.", ErrorCodes.NOT_FOUND);

        // 2. Find the cart item
        var item = cart.Items.FirstOrDefault(i => i.Id == cmd.CartItemId);
        if (item is null)
            return Result<CartDto>.Failure(
                "Cart item not found.", ErrorCodes.NOT_FOUND);

        // 3. Quantity = 0 means remove
        if (cmd.Quantity == 0)
        {
            cart.RemoveItem(cmd.CartItemId);
        }
        else
        {
            // Validate new quantity against current stock
            var stock = await _db.Products
                .Where(p => p.Id == item.ProductId)
                .Select(p => p.StockQuantity)
                .FirstOrDefaultAsync(ct);

            if (cmd.Quantity > stock)
                return Result<CartDto>.Failure(
                    $"Only {stock} unit(s) in stock.",
                    ErrorCodes.INSUFFICIENT_STOCK);

            cart.UpdateItemQuantity(cmd.CartItemId, cmd.Quantity);
        }

        cart.Recalculate();
        await _db.SaveChangesAsync(ct);

        return Result<CartDto>.Success(CartMapper.ToDto(cart));
    }
}