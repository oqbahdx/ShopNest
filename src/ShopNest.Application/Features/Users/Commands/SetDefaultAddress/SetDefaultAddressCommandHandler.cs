using MediatR;

namespace ShopNest.Application.Features.Users.Commands.SetDefaultAddress;

public sealed class SetDefaultAddressCommandHandler
    : IRequestHandler<SetDefaultAddressCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public SetDefaultAddressCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        SetDefaultAddressCommand cmd, CancellationToken ct)
    {
        var userId = _currentUser.UserId;

        // Load all user addresses in one query
        var addresses = await _db.Addresses
            .Where(a => a.UserId == userId)
            .ToListAsync(ct);

        var target = addresses.FirstOrDefault(a => a.Id == cmd.Id);

        if (target is null)
            return Result.Failure("Address not found.", ErrorCodes.NOT_FOUND);

        // Unset current default, set the target
        foreach (var a in addresses)
            a.SetDefault(a.Id == cmd.Id);

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}