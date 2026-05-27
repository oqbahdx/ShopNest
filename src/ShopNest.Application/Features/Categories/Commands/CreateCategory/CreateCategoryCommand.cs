using MediatR;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Categories.Commands.CreateCategory;

public sealed record CreateCategoryCommand(
    string Name,
    string? Description,
    string? ImageUrl,
    int DisplayOrder,
    Guid? ParentCategoryId
) : IRequest<Result<Guid>>;