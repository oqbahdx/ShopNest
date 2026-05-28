using MediatR;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Categories.Commands.DeleteCategory;

public sealed record DeleteCategoryCommand(Guid Id) : IRequest<Result>;
