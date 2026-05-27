using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;

namespace ShopNest.Application.Features.Products.Queries.SearchProducts;

/// <summary>
/// Dedicated full-text search across Name + Description + SKU.
/// Phase 1: EF.Functions.Like()
/// Phase 8 upgrade: EF.Functions.Contains() with SQL Server FULLTEXT INDEX.
/// </summary>
public sealed record SearchProductsQuery(
    string Search,
    int    Page     = 1,
    int    PageSize = 20
) : IRequest<Result<PagedResult<ProductListDto>>>;