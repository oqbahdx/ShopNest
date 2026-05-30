using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Cart.DTOs;

namespace ShopNest.Application.Features.Wishlist.Queries.GetWishlist;

public sealed class GetWishlistQueryHandler
    : IRequestHandler<GetWishlistQuery, Result<WishlistDto>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetWishlistQueryHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<WishlistDto>> Handle(
        GetWishlistQuery _, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        var wishlist = await _db.Wishlists
            .Include(w => w.Items)
            .ThenInclude(i => i.Product)
            .ThenInclude(p => p.Images)
            .FirstOrDefaultAsync(w => w.UserId == userId, ct);

        if (wishlist is null)
        {
            // Return empty wishlist without persisting — auto-create on first Add
            return Result<WishlistDto>.Success(
                new WishlistDto(Guid.Empty, []));
        }

        var dto = new WishlistDto(
            Id: wishlist.Id,
            Items: wishlist.Items.Select(i => new WishlistItemDto(
                ProductId: i.ProductId,
                ProductName: i.Product.Name,
                ProductSlug: i.Product.Slug,
                PrimaryImageUrl: i.Product.Images
                    .FirstOrDefault(img => img.IsPrimary)?.ImageUrl,
                Price: i.Product.Price,
                IsInStock: i.Product.StockQuantity > 0,
                AddedAt: i.AddedAt
            )).OrderByDescending(i => i.AddedAt).ToList()
        );

        return Result<WishlistDto>.Success(dto);
    }
}