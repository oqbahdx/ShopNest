using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;

namespace ShopNest.Application.Features.Products.Queries.GetProducts;

public sealed record GetProductsQuery(
    string? Search = null,
    Guid? CategoryId = null,
    decimal? MinPrice = null,
    decimal? MaxPrice = null,
    bool? IsFeatured = null,
    int Page = 1,
    int PageSize = 10,
    string SortBy = "createdAt",
    string SortOrder = "desc") : IRequest<Result<PagedResult<ProductDto>>>;
