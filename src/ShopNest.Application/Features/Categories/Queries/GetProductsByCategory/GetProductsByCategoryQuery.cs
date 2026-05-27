using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;

namespace ShopNest.Application.Features.GetCategories.Queries.GetProductsByCategory;

/// <summary>
/// Returns paginated products for a category AND all its sub-categories.
/// </summary>
public sealed record GetProductsByCategoryQuery(
    Guid CategoryId,
    int Page = 1,
    int PageSize = 20,
    string? SortBy = "createdAt",
    string? SortOrder = "desc"
) : IRequest<Result<PagedResult<ProductListDto>>>;