using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Products.Commands.DeleteProductImage;

public sealed class DeleteProductImageCommandHandler : IRequestHandler<DeleteProductImageCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly IFileService _fileService;

    public DeleteProductImageCommandHandler(IApplicationDbContext db, IFileService fileService)
    {
        _db = db;
        _fileService = fileService;
    }

    public async Task<Result> Handle(DeleteProductImageCommand request, CancellationToken ct)
    {
        var image = await _db.ProductImages
            .FirstOrDefaultAsync(i => i.Id == request.ImageId && i.ProductId == request.ProductId, ct);
        if (image is null)
            return Result.Failure("Product image not found.", ErrorCodes.NotFound);

        var images = await _db.ProductImages
            .Where(i => i.ProductId == request.ProductId)
            .OrderBy(i => i.DisplayOrder)
            .ThenBy(i => i.Id)
            .ToListAsync(ct);

        if (images.Count <= 1)
            return Result.Failure("Cannot delete the only product image.", ErrorCodes.Conflict);

        var wasPrimary = image.IsPrimary;
        _db.ProductImages.Remove(image);

        if (wasPrimary)
        {
            var replacement = images.First(i => i.Id != image.Id);
            replacement.SetPrimary(true);
        }

        await _fileService.DeleteAsync(image.ImageUrl, "products", ct);
        await _db.SaveChangesAsync(ct);

        return Result.Success();
    }
}
