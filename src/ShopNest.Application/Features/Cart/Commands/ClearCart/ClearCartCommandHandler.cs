using MediatR;

namespace ShopNest.Application.Features.Cart.Commands.ClearCart;

public sealed class ClearCartCommandHandler
    : IRequestHandler<ClearCartCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public ClearCartCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        ClearCartCommand _, CancellationToken ct)
    {
        var cart = await _db.Carts
            .Include(c => c.Items)
            .FirstOrDefaultAsync(
                c => c.UserId == _currentUser.UserId, ct);

        if (cart is null)
            return Result.Success(); // idempotent — no cart = already empty

        cart.Clear(); // removes all items and applied coupon
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}