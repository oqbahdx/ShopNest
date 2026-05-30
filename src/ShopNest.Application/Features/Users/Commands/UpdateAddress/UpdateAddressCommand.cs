using MediatR;

namespace ShopNest.Application.Features.Users.Commands.UpdateAddress;

public sealed record UpdateAddressCommand(
    Guid Id,
    string FullName,
    string Line1,
    string? Line2,
    string City,
    string State,
    string PostalCode,
    string Country,
    string? Phone
) : IRequest<Result>;

public sealed class UpdateAddressCommandHandler
    : IRequestHandler<UpdateAddressCommand, Result>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateAddressCommandHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(
        UpdateAddressCommand cmd, CancellationToken ct)
    {
        var address = await _db.Addresses
            .FirstOrDefaultAsync(a => a.Id == cmd.Id, ct);

        if (address is null)
            return Result.Failure("Address not found.", ErrorCodes.NOT_FOUND);

        // Ownership check
        if (address.UserId != _currentUser.UserId)
            return Result.Failure("Access denied.", ErrorCodes.FORBIDDEN);

        address.Update(
            cmd.FullName, cmd.Line1, cmd.Line2,
            cmd.City, cmd.State, cmd.PostalCode,
            cmd.Country, cmd.Phone);

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}