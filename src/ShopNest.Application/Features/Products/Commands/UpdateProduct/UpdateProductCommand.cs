using MediatR;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Products.Commands.UpdateProduct;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string? Description,
    string? ShortDescription,
    string Sku,
    decimal Price,
    decimal? CompareAtPrice,
    decimal CostPrice,
    decimal? Weight,
    int LowStockThreshold,
    bool IsFeatured,
    bool IsActive,
    Guid CategoryId
) : IRequest<Result>;