using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopNest.Application.Common.Helpers;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IApplicationDbContext _db;

    public UpdateProductCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == request.Id, ct);
        if (product is null)
            return Result.Failure("Product not found.", ErrorCodes.NotFound);

        var categoryExists = await _db.Categories
            .AnyAsync(c => c.Id == request.CategoryId && c.IsActive, ct);
        if (!categoryExists)
            return Result.Failure("Category not found.", ErrorCodes.NotFound);

        var skuTaken = await _db.Products
            .AnyAsync(p => p.SKU == request.Sku && p.Id != request.Id, ct);
        if (skuTaken)
            return Result.Failure("SKU already exists.", ErrorCodes.Conflict);

        var nameChanged = !string.Equals(product.Name, request.Name, StringComparison.Ordinal);
        var slug = nameChanged
            ? await GenerateUniqueProductSlugAsync(request.Name, request.Id, ct)
            : product.Slug;

        product.Update(
            request.Name.Trim(),
            request.Description,
            request.ShortDescription,
            request.Sku.Trim(),
            slug,
            request.Price,
            request.CompareAtPrice,
            request.CostPrice,
            request.Weight,
            request.LowStockThreshold,
            request.IsFeatured,
            request.IsActive,
            request.CategoryId);

        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }

    private async Task<string> GenerateUniqueProductSlugAsync(string name, Guid currentProductId, CancellationToken ct)
    {
        var baseSlug = SlugHelper.FromText(name);
        var slug = baseSlug;
        var counter = 1;

        while (await _db.Products.AnyAsync(p => p.Slug == slug && p.Id != currentProductId, ct))
            slug = $"{baseSlug}-{counter++}";

        return slug;
    }
}
