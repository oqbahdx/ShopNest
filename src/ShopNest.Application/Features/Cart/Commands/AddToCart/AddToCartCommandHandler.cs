using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopNest.Application.Features.Cart.DTOs;
using ShopNest.Application.Features.Cart.Mappers;
using CartEntity = ShopNest.Domain.Entities.Cart;

namespace ShopNest.Application.Features.Cart.Commands.AddToCart;

public sealed class AddToCartCommandHandler
    : IRequestHandler<AddToCartCommand, Result<CartDto>>
{
    private readonly IAppDbContext       _db;
    private readonly ICurrentUserService _currentUser;

    public AddToCartCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db          = db;
        _currentUser = currentUser;
    }

    public async Task<Result<CartDto>> Handle(
        AddToCartCommand cmd, CancellationToken ct)
    {
        // 1. Validate product exists and has sufficient stock
        var product = await _db.Products
            .FirstOrDefaultAsync(p => p.Id == cmd.ProductId, ct);

        if (product is null)
            return Result<CartDto>.Failure(
                "Product not found.", ErrorCodes.NOT_FOUND);

        if (product.StockQuantity < cmd.Quantity)
            return Result<CartDto>.Failure(
                $"Only {product.StockQuantity} unit(s) in stock.",
                ErrorCodes.INSUFFICIENT_STOCK);

        if (_currentUser.UserId is not Guid userId)
            return Result<CartDto>.Failure(
                "Authentication required.", ErrorCodes.FORBIDDEN);

        // 2. Load or auto-create cart for current user
        var cart = await _db.Carts
            .Include(c => c.Items).ThenInclude(i => i.Product)
                .ThenInclude(p => p.Images)
            .Include(c => c.Coupon)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);

        if (cart is null)
        {
            cart = CartEntity.Create(userId);
            _db.Carts.Add(cart);
        }

        // 3. AddItem merges if product already in cart
        try
        {
            cart.AddItem(cmd.ProductId, product.Price, cmd.Quantity);

            // CRITICAL: Always call Recalculate after any cart mutation
            cart.Recalculate();

            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException)
        {
            // DB unique constraint on (CartId, ProductId) catches race condition
            return Result<CartDto>.Failure(
                "Could not add item. Please try again.",
                ErrorCodes.CONFLICT);
        }

        return Result<CartDto>.Success(CartMapper.ToDto(cart));
    }
}