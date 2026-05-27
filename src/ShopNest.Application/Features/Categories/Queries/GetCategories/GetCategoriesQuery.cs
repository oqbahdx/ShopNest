using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Products.DTOs;

namespace ShopNest.Application.Features.GetCategories.Queries.GetCategories;

/// <summary>
/// Returns the full category tree with sub-categories and product counts.
/// Cached for 60 minutes — invalidated by any category or product mutation.
/// </summary>
public sealed record GetCategoriesQuery : IRequest<Result<List<CategoryDto>>>;