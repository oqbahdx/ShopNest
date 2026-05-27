using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;

namespace ShopNest.Application.Features.Products.Queries.GetFeaturedProducts;

/// <summary>
/// Admin-only query — includes soft-deleted products and internal fields
/// (CostPrice, StockQuantity, LowStockThreshold).
/// Bypasses the global IsDeleted query filter via IgnoreQueryFilters().
/// </summary>
public sealed record GetAdminProductsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    Guid? CategoryId = null,
    bool? IsActive = null,
    bool? IsDeleted = null,
    bool? LowStock = null,
    string SortBy = "createdAt",
    string SortOrder = "desc"
) : IRequest<Result<PagedResult<AdminProductDto>>>;