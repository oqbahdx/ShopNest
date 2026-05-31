using MediatR;
using ShopNest.Application.Common.Cache;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;
using ShopNest.Domain.Entities;

namespace ShopNest.Application.Features.Products.Commands.UploadProductImages;

public sealed class UploadProductImagesCommandHandler
    : IRequestHandler<UploadProductImagesCommand, Result<List<string>>>
{
    private readonly IApplicationDbContext _db;
    private readonly IFileService _fileService;
    private readonly ICacheService _cache;

    public UploadProductImagesCommandHandler(
        IApplicationDbContext db, IFileService fileService, ICacheService cache)
    {
        _db = db;
        _fileService = fileService;
        _cache = cache;
    }

    public async Task<Result<List<string>>> Handle(
        UploadProductImagesCommand cmd, CancellationToken ct)
    {
        // 1. Load product with existing images
        var product = await _db.Products
            .Include(p => p.Images)
            .FirstOrDefaultAsync(p => p.Id == cmd.ProductId, ct);
        if (product is null)
            return Result<List<string>>.Failure(
                "Product not found.", ErrorCodes.NOT_FOUND);
        // 2. Guard: max 10 images per product
        var remainingSlots = 10 - product.Images.Count;
        if (cmd.Images.Count > remainingSlots)
            return Result<List<string>>.Failure(
                $"Cannot upload {cmd.Images.Count} image(s). " +
                $"Product already has {product.Images.Count} — max is 10.",
                ErrorCodes.CONFLICT);
        // 3. Upload files and create ProductImage entities
        var uploadedUrls = new List<string>();
        var nextOrder = product.Images.Any()
            ? product.Images.Max(i => i.DisplayOrder) + 1
            : 0;
        var isFirstBatch = !product.Images.Any();
        for (int i = 0; i < cmd.Images.Count; i++)
        {
            var result = await _fileService.UploadAsync(
                cmd.Images[i], "products", ct);
            // First image of a brand-new product becomes the primary
            var isPrimary = isFirstBatch && i == 0;
            var image = ProductImage.Create(
                productId: cmd.ProductId,
                url: result.Url,
                altText: Path.GetFileNameWithoutExtension(result.FileName),
                displayOrder: nextOrder + i,
                isPrimary: isPrimary
            );
            _db.ProductImages.Add(image);
            uploadedUrls.Add(result.Url);
        }

        await _db.SaveChangesAsync(ct);
        await _cache.RemoveByPrefixAsync(CacheKeys.Products.Prefix, ct);
        await _cache.RemoveByPrefixAsync(CacheKeys.Categories.Prefix, ct);
        return Result<List<string>>.Success(uploadedUrls);
    }
}
