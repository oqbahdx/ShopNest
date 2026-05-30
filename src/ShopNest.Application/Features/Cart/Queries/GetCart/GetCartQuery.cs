using MediatR;
using ShopNest.Application.Features.Cart.DTOs;

namespace ShopNest.Application.Features.Cart.Queries.GetCart;

/// <summary>
/// Returns the current user's cart. Auto-creates if none exists.
/// </summary>
public sealed record GetCartQuery : IRequest<Result<CartDto>>;