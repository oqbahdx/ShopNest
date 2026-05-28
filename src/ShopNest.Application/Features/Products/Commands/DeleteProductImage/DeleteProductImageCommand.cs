using MediatR;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Products.Commands.DeleteProductImage;

public sealed record DeleteProductImageCommand(Guid ProductId, Guid ImageId) : IRequest<Result>;
