using MediatR;

namespace ShopNest.Application.Features.Wishlist.Commands.RemoveFromWishlist;

public sealed class RemoveFromWishlistCommandHandler
    : IRequestHandler<RemoveFromWishlistCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public RemoveFromWishlistCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        RemoveFromWishlistCommand cmd, CancellationToken ct)
    {
        var wishlist = await _db.Wishlists
            .Include(w => w.Items)
            .FirstOrDefaultAsync(
                w => w.UserId == _currentUser.UserId, ct);

        if (wishlist is null)
            return Result.Failure("Wishlist not found.", ErrorCodes.NOT_FOUND);

        try
        {
            wishlist.RemoveItem(cmd.ProductId);
            await _db.SaveChangesAsync(ct);
        }
        catch (DomainException ex)
        {
            return Result.Failure(ex.Message, ErrorCodes.NOT_FOUND);
        }

        return Result.Success();
    }
}