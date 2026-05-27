using MediatR;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Products.Commands.DeleteProductImage;

public sealed class DeleteProductImageCommandHandler
    : IRequestHandler<DeleteProductImageCommand, Result>
{
    private readonly IApplicationDbContext _db;
    private readonly IFileService _fileService;

    public DeleteProductImageCommandHandler(
        IApplicationDbContext db, IFileService fileService)
    {
        _db = db;
        _fileService = fileService;
    }

    public async Task<Result> Handle(
        DeleteProductImageCommand cmd, CancellationToken ct)
    {
        // 1. Load all images for this product
        var images = await _db.ProductImages
            .Where(i => i.ProductId == cmd.ProductId)
            .ToListAsync(ct);
        if (!images.Any())
            return Result.Failure(
                "Product not found or has no images.", ErrorCodes.NOT_FOUND);
        // 2. Locate target image
        var target = images.FirstOrDefault(i => i.Id == cmd.ImageId);
        if (target is null)
            return Result.Failure(
                "Image not found or does not belong to this product.",
                ErrorCodes.NOT_FOUND);
        // 3. Guard: cannot delete the sole image
        if (images.Count == 1)
            return Result.Failure(
                "Cannot delete the only image of a product. " +
                "Upload a replacement first.",
                ErrorCodes.CONFLICT);
        // 4. If deleting primary, auto-promote the next image by display order
        if (target.IsPrimary)
        {
            var next = images
                .Where(i => i.Id != cmd.ImageId)
                .OrderBy(i => i.DisplayOrder)
                .First();
            next.SetPrimary(true);
        }

        // 5. Remove from storage then DB
        await _fileService.DeleteAsync(target.ImageUrl, "products", ct);
        _db.ProductImages.Remove(target);
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}