using MediatR;
using ShopNest.Domain.Entities;

namespace ShopNest.Application.Features.Users.Commands.AddAddress;

public sealed class AddAddressCommandHandler
    : IRequestHandler<AddAddressCommand, Result<Guid>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private const int MaxAddresses = 10;

    public AddAddressCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(
        AddAddressCommand cmd, CancellationToken ct)
    {
        if (_currentUser.UserId is not Guid userId)
            return Result<Guid>.Failure("Authentication required.", ErrorCodes.FORBIDDEN);

        // 1. Enforce max 10 addresses per user
        var count = await _db.Addresses
            .CountAsync(a => a.UserId == userId, ct);

        if (count >= MaxAddresses)
            return Result<Guid>.Failure(
                $"A maximum of {MaxAddresses} addresses is allowed.",
                ErrorCodes.CONFLICT);

        // 2. If SetAsDefault, unset the current default first
        if (cmd.SetAsDefault)
        {
            var current = await _db.Addresses
                .Where(a => a.UserId == userId && a.IsDefault)
                .ToListAsync(ct);

            foreach (var a in current) a.SetDefault(false);
        }

        // 3. Create the new address
        var address = Address.Create(
            userId: userId,
            fullName: cmd.FullName,
            line1: cmd.Line1,
            line2: cmd.Line2,
            city: cmd.City,
            state: cmd.State,
            postalCode: cmd.PostalCode,
            country: cmd.Country,
            phone: cmd.Phone
        );

        // First ever address is always default
        if (cmd.SetAsDefault || count == 0)
            address.SetDefault(true);

        _db.Addresses.Add(address);
        await _db.SaveChangesAsync(ct);

        return Result<Guid>.Success(address.Id);
    }
}
