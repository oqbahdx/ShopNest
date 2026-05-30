using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Cart.DTOs;

namespace ShopNest.Application.Features.Coupons.Queries.GetCoupons;

/// <summary>Admin: paginated coupon list with usage stats.</summary>
public sealed record GetCouponsQuery(
    int    Page      = 1,
    int    PageSize  = 20,
    bool?  IsActive  = null,
    string SortBy    = "createdAt",
    string SortOrder = "desc"
) : IRequest<Result<PagedResult<CouponDto>>>;