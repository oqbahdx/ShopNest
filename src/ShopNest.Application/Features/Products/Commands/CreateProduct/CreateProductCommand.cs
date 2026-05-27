using MediatR;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string? Description,
    string? ShortDescription,
    string Sku,
    decimal Price,
    decimal? CompareAtPrice,
    decimal CostPrice,
    decimal? Weight,
    int StockQuantity,
    int LowStockThreshold,
    bool IsFeatured,
    Guid CategoryId
) : IRequest<Result<Guid>>;