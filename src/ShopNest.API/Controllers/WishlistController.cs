using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopNest.Application.Features.Wishlist.Commands.AddToWishlist;
using ShopNest.Application.Features.Wishlist.Commands.RemoveFromWishlist;
using ShopNest.Application.Features.Wishlist.Queries.GetWishlist;

namespace ShopNest.API.Controllers;

[Authorize]
[Route("api/v1/wishlist")]
public sealed class WishlistController : BaseApiController
{
    /// GET /api/v1/wishlist
    [HttpGet]
    public async Task<IActionResult> GetWishlist(CancellationToken ct = default)
        => ToResponse(await Mediator.Send(new GetWishlistQuery(), ct));

    /// POST /api/v1/wishlist
    [HttpPost]
    public async Task<IActionResult> AddToWishlist(
        [FromBody] AddToWishlistRequest req, CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new AddToWishlistCommand(req.ProductId), ct));

    /// DELETE /api/v1/wishlist/{productId}
    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> RemoveFromWishlist(
        Guid productId, CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new RemoveFromWishlistCommand(productId), ct));
}

public sealed record AddToWishlistRequest(Guid ProductId);