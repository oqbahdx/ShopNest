using MediatR;
using ShopNest.Application.Features.Cart.DTOs;

namespace ShopNest.Application.Features.Wishlist.Queries.ValidateCoupon;

/// <summary>
/// Public endpoint — lets the client check a coupon's validity
/// and preview the discount before applying it to the cart.
/// </summary>
public sealed record ValidateCouponQuery(
    string  Code,
    decimal Subtotal
) : IRequest<Result<CouponValidationDto>>;