using MediatR;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Categories.Commands.UpdateCategory;

public sealed record UpdateCategoryCommand(
    Guid Id,
    string Name,
    string? Description,
    string? ImageUrl,
    int DisplayOrder,
    Guid? ParentCategoryId
) : IRequest<Result>;