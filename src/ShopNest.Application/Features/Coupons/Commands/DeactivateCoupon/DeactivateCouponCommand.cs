using MediatR;

namespace ShopNest.Application.Features.Coupons.Commands.DeactivateCoupon;

public sealed record DeactivateCouponCommand(Guid Id) : IRequest<Result>;