using MediatR;
using WishlistEntity = ShopNest.Domain.Entities.Wishlist;

namespace ShopNest.Application.Features.Wishlist.Commands.AddToWishlist;

public sealed class AddToWishlistCommandHandler
    : IRequestHandler<AddToWishlistCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public AddToWishlistCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        AddToWishlistCommand cmd, CancellationToken ct)
    {
        if (_currentUser.UserId is not Guid userId)
            return Result.Failure(
                "Authentication required.", ErrorCodes.FORBIDDEN);

        // 1. Verify product exists
        var productExists = await _db.Products
            .AnyAsync(p => p.Id == cmd.ProductId, ct);

        if (!productExists)
            return Result.Failure("Product not found.", ErrorCodes.NOT_FOUND);

        // 2. Load or create wishlist
        var wishlist = await _db.Wishlists
            .Include(w => w.Items)
            .FirstOrDefaultAsync(w => w.UserId == userId, ct);

        if (wishlist is null)
        {
            wishlist = WishlistEntity.Create(userId);
            _db.Wishlists.Add(wishlist);
        }

        // 3. AddItem enforces max 100 and no duplicates via domain method
        try
        {
            wishlist.AddItem(cmd.ProductId);
            await _db.SaveChangesAsync(ct);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ErrorCodes.CONFLICT);
        }

        return Result.Success();
    }
}