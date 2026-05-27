using MediatR;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Products.Commands.SetPrimaryImage;

public sealed record SetPrimaryImageCommand(
    Guid ProductId,
    Guid ImageId
) : IRequest<Result>;