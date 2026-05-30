using MediatR;

namespace ShopNest.Application.Features.Cart.Commands.ClearCart;

public sealed record ClearCartCommand : IRequest<Result>;