using MediatR;
using ShopNest.Application.Features.Cart.DTOs;

namespace ShopNest.Application.Features.Cart.Commands.RemoveCartItem;

public sealed record RemoveCartItemCommand(Guid CartItemId)
    : IRequest<Result<CartDto>>;