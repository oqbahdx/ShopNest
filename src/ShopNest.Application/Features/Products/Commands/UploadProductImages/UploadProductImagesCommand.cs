using MediatR;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Products.Commands.UploadProductImages;

public sealed record UploadProductImagesCommand(
    Guid ProductId,
    IReadOnlyList<IFormFile> Images
) : IRequest<Result<List<string>>>;