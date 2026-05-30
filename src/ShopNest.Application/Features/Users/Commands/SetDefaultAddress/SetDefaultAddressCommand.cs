using MediatR;

namespace ShopNest.Application.Features.Users.Commands.SetDefaultAddress;

public sealed record SetDefaultAddressCommand(Guid Id) : IRequest<Result>;
