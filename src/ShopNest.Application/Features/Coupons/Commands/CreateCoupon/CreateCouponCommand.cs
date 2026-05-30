using MediatR;
using ShopNest.Domain.Enums;

namespace ShopNest.Application.Features.Coupons.Commands.CreateCoupon;

public sealed record CreateCouponCommand(
    string Code,
    DiscountType DiscountType,
    decimal DiscountValue,
    decimal? MinimumOrderAmount,
    decimal? MaximumDiscountAmount,
    int? UsageLimit,
    DateTime? ExpiresAt
) : IRequest<Result<Guid>>;