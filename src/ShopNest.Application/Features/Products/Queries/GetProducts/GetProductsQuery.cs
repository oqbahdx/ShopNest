using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;

namespace ShopNest.Application.Features.Products.Queries.GetProducts;

public sealed record GetProductsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    Guid? CategoryId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    bool? InStock = null,
    bool? Featured = null,
    decimal? MinRating = null,
    string SortBy = "createdAt", // name | price | rating | createdAt
    string SortOrder = "desc" // asc | desc
) : IRequest<Result<PagedResult<ProductListDto>>>;
