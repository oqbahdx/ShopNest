using MediatR;
using ShopNest.Application.Features.Cart.DTOs;

namespace ShopNest.Application.Features.Wishlist.Queries.GetWishlist;

public sealed record GetWishlistQuery : IRequest<Result<WishlistDto>>;