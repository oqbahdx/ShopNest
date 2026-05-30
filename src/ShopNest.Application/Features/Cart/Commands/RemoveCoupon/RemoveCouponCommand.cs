using MediatR;
using ShopNest.Application.Features.Cart.DTOs;

namespace ShopNest.Application.Features.Cart.Commands.RemoveCoupon;

public sealed record RemoveCouponCommand : IRequest<Result<CartDto>>;