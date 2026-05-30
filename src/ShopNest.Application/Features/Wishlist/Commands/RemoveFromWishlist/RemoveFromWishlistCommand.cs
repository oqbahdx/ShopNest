using MediatR;

namespace ShopNest.Application.Features.Wishlist.Commands.RemoveFromWishlist;

public sealed record RemoveFromWishlistCommand(Guid ProductId)
    : IRequest<Result>;