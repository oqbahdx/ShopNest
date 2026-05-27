using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;

namespace ShopNest.Application.Features.Products.Queries.GetProductBySlug;

public sealed record GetProductBySlugQuery(string Slug)
    : IRequest<Result<ProductDto>>;