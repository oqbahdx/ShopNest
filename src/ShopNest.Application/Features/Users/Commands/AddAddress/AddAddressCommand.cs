using MediatR;

namespace ShopNest.Application.Features.Users.Commands.AddAddress;

public sealed record AddAddressCommand(
    string FullName,
    string Line1,
    string? Line2,
    string City,
    string State,
    string PostalCode,
    string Country,
    string? Phone,
    bool SetAsDefault
) : IRequest<Result<Guid>>;