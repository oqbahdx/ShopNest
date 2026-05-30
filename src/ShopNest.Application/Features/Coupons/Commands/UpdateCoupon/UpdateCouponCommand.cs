using MediatR;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Coupons.Commands.UpdateCoupon;

public sealed record UpdateCouponCommand(
    Guid         Id,
    DiscountType DiscountType,
    decimal      DiscountValue,
    decimal?     MinimumOrderAmount,
    decimal?     MaximumDiscountAmount,
    int?         UsageLimit,
    DateTime?    ExpiresAt
) : IRequest<Result>;