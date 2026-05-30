using MediatR;
using ShopNest.Application.Features.Cart.DTOs;

namespace ShopNest.Application.Features.Cart.Commands.ApplyCoupon;

public sealed record ApplyCouponCommand(string Code)
    : IRequest<Result<CartDto>>;