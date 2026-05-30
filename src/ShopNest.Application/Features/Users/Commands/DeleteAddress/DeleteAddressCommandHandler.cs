using MediatR;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Users.Commands.DeleteAddress;

public sealed record DeleteAddressCommand(Guid Id) : IRequest<Result>;

public sealed class DeleteAddressCommandHandler
    : IRequestHandler<DeleteAddressCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteAddressCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        DeleteAddressCommand cmd, CancellationToken ct)
    {
        var address = await _db.Addresses
            .FirstOrDefaultAsync(a => a.Id == cmd.Id, ct);

        if (address is null)
            return Result.Failure("Address not found.", ErrorCodes.NOT_FOUND);

        if (address.UserId != _currentUser.UserId)
            return Result.Failure("Access denied.", ErrorCodes.FORBIDDEN);

        // Guard: cannot delete an address that is used by active orders
        var usedByActiveOrder = await _db.Orders
            .AnyAsync(o =>
                o.UserId == _currentUser.UserId &&
                o.ShippingAddressId == cmd.Id &&
                (o.Status == OrderStatus.Pending ||
                 o.Status == OrderStatus.Confirmed ||
                 o.Status == OrderStatus.Processing), ct);

        if (usedByActiveOrder)
            return Result.Failure(
                "This address is used by an active order and cannot be deleted.",
                ErrorCodes.CONFLICT);

        _db.Addresses.Remove(address); // ISoftDeletable intercept in SaveChangesAsync
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}