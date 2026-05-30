using MediatR;
using ShopNest.Application.Features.Cart.DTOs;

namespace ShopNest.Application.Features.Cart.Commands.AddToCart;

public sealed record AddToCartCommand(
    Guid ProductId,
    int  Quantity
) : IRequest<Result<CartDto>>;