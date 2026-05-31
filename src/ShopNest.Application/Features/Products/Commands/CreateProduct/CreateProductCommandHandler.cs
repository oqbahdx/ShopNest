using MediatR;
using ShopNest.Application.Common.Cache;
using ShopNest.Application.Common.Extensions;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;
using ShopNest.Domain.Entities;

namespace ShopNest.Application.Features.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _db;
    private readonly ICacheService _cache;
    public CreateProductCommandHandler(IApplicationDbContext db, ICacheService cache)
    {
        _db = db;
        _cache = cache;
    }

    public async Task<Result<Guid>> Handle(
        CreateProductCommand cmd, CancellationToken ct)
    {
        // 1. Validate category exists
        var categoryExists = await _db.Categories
            .AnyAsync(c => c.Id == cmd.CategoryId, ct);
        if (!categoryExists)
            return Result<Guid>.Failure(
                "Category not found.", ErrorCodes.NOT_FOUND);
        // 2. Validate SKU uniqueness
        var skuTaken = await _db.Products
            .AnyAsync(p => p.SKU == cmd.Sku, ct);
        if (skuTaken)
            return Result<Guid>.Failure(
                "A product with this SKU already exists.", ErrorCodes.CONFLICT);
        // 3. Generate unique slug
        var slug = await GenerateUniqueSlugAsync(cmd.Name.ToSlug(), null, ct);
        // 4. Create product entity
        var product = Product.Create(
            name: cmd.Name,
            description: cmd.Description,
            shortDescription: cmd.ShortDescription,
            sku: cmd.Sku,
            slug: slug,
            price: cmd.Price,
            compareAtPrice: cmd.CompareAtPrice,
            costPrice: cmd.CostPrice,
            weight: cmd.Weight,
            stockQuantity: cmd.StockQuantity,
            lowStockThreshold: cmd.LowStockThreshold,
            isFeatured: cmd.IsFeatured,
            categoryId: cmd.CategoryId
        );
        _db.Products.Add(product);
        await _db.SaveChangesAsync(ct);
        await _cache.RemoveByPrefixAsync(CacheKeys.Products.Prefix, ct);
        await _cache.RemoveByPrefixAsync(CacheKeys.Categories.Prefix, ct);
        return Result<Guid>.Success(product.Id);
    }

    private async Task<string> GenerateUniqueSlugAsync(
        string baseSlug, Guid? excludeId, CancellationToken ct)
    {
        var slug = baseSlug;
        var counter = 1;
        while (await _db.Products.AnyAsync(
                   p => p.Slug == slug && p.Id != excludeId, ct))
        {
            slug = $"{baseSlug}-{counter++}";
        }

        return slug;
    }
}
