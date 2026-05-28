using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;

namespace ShopNest.Application.Features.Products.Queries.GetFeaturedProducts;

public sealed record GetFeaturedProductsQuery(int Top = 8) : IRequest<Result<IReadOnlyList<ProductDto>>>;
