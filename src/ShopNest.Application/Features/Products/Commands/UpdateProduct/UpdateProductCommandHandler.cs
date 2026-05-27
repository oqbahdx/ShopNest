using MediatR;
using ShopNest.Application.Common.Extensions;
using ShopNest.Application.Common.Models;

namespace ShopNest.Application.Features.Products.Commands.UpdateProduct;

public sealed class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IApplicationDbContext _db;
    public UpdateProductCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result> Handle(
        UpdateProductCommand cmd, CancellationToken ct)
    {
        // 1. Load product
        var product = await _db.Products.FindAsync(
            new object[] { cmd.Id }, ct);
        if (product is null)
            return Result.Failure("Product not found.", ErrorCodes.NOT_FOUND);
        // 2. Validate category
        var categoryExists = await _db.Categories
            .AnyAsync(c => c.Id == cmd.CategoryId, ct);
        if (!categoryExists)
            return Result.Failure("Category not found.", ErrorCodes.NOT_FOUND);
        // 3. Validate SKU uniqueness (exclude self)
        var skuTaken = await _db.Products
            .AnyAsync(p => p.SKU == cmd.Sku && p.Id != cmd.Id, ct);
        if (skuTaken)
            return Result.Failure(
                "A product with this SKU already exists.", ErrorCodes.CONFLICT);
        // 4. Regenerate slug only if name changed
        var slug = product.Slug;
        if (!product.Name.Equals(cmd.Name, StringComparison.OrdinalIgnoreCase))
        {
            var baseSlug = cmd.Name.ToSlug();
            slug = baseSlug;
            var counter = 1;
            while (await _db.Products.AnyAsync(
                       p => p.Slug == slug && p.Id != cmd.Id, ct))
            {
                slug = $"{baseSlug}-{counter++}";
            }
        }

        // 5. Apply changes via entity method
        product.Update(
            name: cmd.Name,
            description: cmd.Description,
            shortDescription: cmd.ShortDescription,
            sku: cmd.Sku,
            slug: slug,
            price: cmd.Price,
            compareAtPrice: cmd.CompareAtPrice,
            costPrice: cmd.CostPrice,
            weight: cmd.Weight,
            lowStockThreshold: cmd.LowStockThreshold,
            isFeatured: cmd.IsFeatured,
            isActive: cmd.IsActive,
            categoryId: cmd.CategoryId
        );
        await _db.SaveChangesAsync(ct);
        return Result.Success();
    }
}
