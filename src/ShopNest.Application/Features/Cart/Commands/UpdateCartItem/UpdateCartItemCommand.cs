using MediatR;
using ShopNest.Application.Features.Cart.DTOs;

namespace ShopNest.Application.Features.Cart.Commands.UpdateCartItem;

/// <summary>
/// Updates quantity of an existing cart item.
/// Quantity = 0 removes the item (convenience rule).
/// </summary>
public sealed record UpdateCartItemCommand(
    Guid CartItemId,
    int  Quantity
) : IRequest<Result<CartDto>>;