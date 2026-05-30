using MediatR;
using ShopNest.Application.Common;
namespace ShopNest.Application.Features.Wishlist.Commands.AddToWishlist;

public sealed record AddToWishlistCommand(Guid ProductId)
    : IRequest<Result>;