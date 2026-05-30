using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopNest.Application.Features.Coupons.Commands.CreateCoupon;
using ShopNest.Application.Features.Coupons.Commands.DeactivateCoupon;
using ShopNest.Application.Features.Coupons.Commands.UpdateCoupon;
using ShopNest.Application.Features.Coupons.Queries.GetCoupons;
using ShopNest.Application.Features.Wishlist.Queries.ValidateCoupon;
using ShopNest.Domain.Enums;

namespace ShopNest.API.Controllers;

[Route("api/v1")]
public sealed class CouponsController : BaseApiController
{
    /// GET /api/v1/coupons/validate?code=SAVE10&subtotal=150
    [HttpGet("coupons/validate")]
    [AllowAnonymous]
    public async Task<IActionResult> Validate(
        [FromQuery] string code,
        [FromQuery] decimal subtotal,
        CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new ValidateCouponQuery(code, subtotal), ct));

    /// GET /api/v1/admin/coupons
    [HttpGet("admin/coupons")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? isActive = null,
        [FromQuery] string sortBy = "createdAt",
        [FromQuery] string sortOrder = "desc",
        CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new GetCouponsQuery(page, pageSize, isActive, sortBy, sortOrder), ct));

    /// POST /api/v1/admin/coupons
    [HttpPost("admin/coupons")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        [FromBody] CreateCouponRequest req, CancellationToken ct = default)
    {
        var result = await Mediator.Send(new CreateCouponCommand(
            req.Code, req.DiscountType, req.DiscountValue,
            req.MinimumOrderAmount, req.MaximumDiscountAmount,
            req.UsageLimit, req.ExpiresAt), ct);

        return ToResponse(result);
    }

    /// PUT /api/v1/admin/coupons/{id}
    [HttpPut("admin/coupons/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCouponRequest req,
        CancellationToken ct = default)
        => ToResponse(await Mediator.Send(new UpdateCouponCommand(
            id, req.DiscountType, req.DiscountValue,
            req.MinimumOrderAmount, req.MaximumDiscountAmount,
            req.UsageLimit, req.ExpiresAt), ct));

    /// DELETE /api/v1/admin/coupons/{id}
    [HttpDelete("admin/coupons/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deactivate(
        Guid id, CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new DeactivateCouponCommand(id), ct));
}

public sealed record CreateCouponRequest(
    string Code,
    DiscountType DiscountType,
    decimal DiscountValue,
    decimal? MinimumOrderAmount,
    decimal? MaximumDiscountAmount,
    int? UsageLimit,
    DateTime? ExpiresAt
);

public sealed record UpdateCouponRequest(
    DiscountType DiscountType,
    decimal DiscountValue,
    decimal? MinimumOrderAmount,
    decimal? MaximumDiscountAmount,
    int? UsageLimit,
    DateTime? ExpiresAt
);