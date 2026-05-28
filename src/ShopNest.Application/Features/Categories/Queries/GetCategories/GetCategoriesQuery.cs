using MediatR;
using ShopNest.Application.Common.Models;
using ShopNest.Application.Features.Categories.DTOs;

namespace ShopNest.Application.Features.Categories.Queries.GetCategories;

public sealed record GetCategoriesQuery : IRequest<Result<IReadOnlyList<CategoryDto>>>;
