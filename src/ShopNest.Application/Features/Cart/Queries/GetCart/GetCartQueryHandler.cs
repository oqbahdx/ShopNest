using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopNest.Application.Features.Cart.DTOs;
using ShopNest.Application.Features.Cart.Mappers;
using CartEntity = ShopNest.Domain.Entities.Cart;

namespace ShopNest.Application.Features.Cart.Queries.GetCart;

public sealed class GetCartQueryHandler
    : IRequestHandler<GetCartQuery, Result<CartDto>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetCartQueryHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<CartDto>> Handle(
        GetCartQuery _, CancellationToken ct)
    {
        if (_currentUser.UserId is not Guid userId)
            return Result<CartDto>.Failure(
                "Authentication required.", ErrorCodes.FORBIDDEN);

        var cart = await _db.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .ThenInclude(p => p.Images)
            .Include(c => c.Coupon)
            .FirstOrDefaultAsync(c => c.UserId == userId, ct);

        // Auto-create empty cart on first access
        if (cart is null)
        {
            cart = CartEntity.Create(userId);
            _db.Carts.Add(cart);
            await _db.SaveChangesAsync(ct);
        }

        return Result<CartDto>.Success(CartMapper.ToDto(cart));
    }
}