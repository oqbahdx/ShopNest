using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopNest.Application.Features.Cart.Commands.AddToCart;
using ShopNest.Application.Features.Cart.Commands.ApplyCoupon;
using ShopNest.Application.Features.Cart.Commands.ClearCart;
using ShopNest.Application.Features.Cart.Commands.RemoveCartItem;
using ShopNest.Application.Features.Cart.Commands.RemoveCoupon;
using ShopNest.Application.Features.Cart.Commands.UpdateCartItem;
using ShopNest.Application.Features.Cart.Queries.GetCart;

namespace ShopNest.API.Controllers;

[Authorize]
[Route("api/v1/cart")]
public sealed class CartController : BaseApiController
{
    /// GET /api/v1/cart
    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken ct = default)
        => ToResponse(await Mediator.Send(new GetCartQuery(), ct));

    /// POST /api/v1/cart/items
    [HttpPost("items")]
    public async Task<IActionResult> AddItem(
        [FromBody] AddToCartCommand cmd, CancellationToken ct = default)
        => ToResponse(await Mediator.Send(cmd, ct));

    /// PUT /api/v1/cart/items/{id}
    [HttpPut("items/{id:guid}")]
    public async Task<IActionResult> UpdateItem(
        Guid id,
        [FromBody] UpdateCartItemRequest req,
        CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new UpdateCartItemCommand(id, req.Quantity), ct));

    /// DELETE /api/v1/cart/items/{id}
    [HttpDelete("items/{id:guid}")]
    public async Task<IActionResult> RemoveItem(
        Guid id, CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new RemoveCartItemCommand(id), ct));

    /// DELETE /api/v1/cart
    [HttpDelete]
    public async Task<IActionResult> ClearCart(CancellationToken ct = default)
        => ToResponse(await Mediator.Send(new ClearCartCommand(), ct));

    /// POST /api/v1/cart/coupon
    [HttpPost("coupon")]
    public async Task<IActionResult> ApplyCoupon(
        [FromBody] ApplyCouponRequest req, CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new ApplyCouponCommand(req.Code), ct));

    /// DELETE /api/v1/cart/coupon
    [HttpDelete("coupon")]
    public async Task<IActionResult> RemoveCoupon(CancellationToken ct = default)
        => ToResponse(await Mediator.Send(new RemoveCouponCommand(), ct));
}

public sealed record UpdateCartItemRequest(int Quantity);

public sealed record ApplyCouponRequest(string Code);