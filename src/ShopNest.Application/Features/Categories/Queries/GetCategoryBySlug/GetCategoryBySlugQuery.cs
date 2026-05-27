using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;

namespace ShopNest.Application.Features.GetCategories.Queries.GetCategoryBySlug;

public sealed record GetCategoryBySlugQuery(string Slug)
    : IRequest<Result<CategoryDto>>;