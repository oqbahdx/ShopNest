using MediatR;
using ShopNest.Application.Features.Users.DTOs;

namespace ShopNest.Application.Features.Notifications.Queries.GetAddresses;

public sealed record GetAddressesQuery : IRequest<Result<List<AddressDto>>>;

public sealed class GetAddressesQueryHandler
    : IRequestHandler<GetAddressesQuery, Result<List<AddressDto>>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetAddressesQueryHandler(
        IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Result<List<AddressDto>>> Handle(
        GetAddressesQuery _, CancellationToken ct)
    {
        var addresses = await _db.Addresses
            .AsNoTracking()
            .Where(a => a.UserId == _currentUser.UserId)
            .OrderByDescending(a => a.IsDefault)
            .ThenBy(a => a.CreatedAt)
            .Select(a => new AddressDto(
                a.Id, a.FullName, a.Street, null,
                a.City, a.State, a.PostalCode, a.Country,
                a.Phone, a.IsDefault))
            .ToListAsync(ct);

        return Result<List<AddressDto>>.Success(addresses);
    }
}
