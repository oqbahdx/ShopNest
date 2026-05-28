using MediatR;
using Microsoft.EntityFrameworkCore;
using ShopNest.Application.Common.Helpers;
using ShopNest.Application.Common.Interfaces;
using ShopNest.Application.Common.Models;
using ShopNest.Domain.Entities;

namespace ShopNest.Application.Features.Products.Commands.CreateProduct;

public sealed class CreateProductCommandHandler
    : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _db;

    public CreateProductCommandHandler(IApplicationDbContext db) => _db = db;

    public async Task<Result<Guid>> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var categoryExists = await _db.Categories
            .AnyAsync(c => c.Id == request.CategoryId && c.IsActive, ct);
        if (!categoryExists)
            return Result<Guid>.Failure("Category not found.", ErrorCodes.NotFound);

        var skuExists = await _db.Products
            .AnyAsync(p => p.SKU == request.Sku, ct);
        if (skuExists)
            return Result<Guid>.Failure("SKU already exists.", ErrorCodes.Conflict);

        var slug = await GenerateUniqueProductSlugAsync(request.Name, null, ct);
        var product = Product.Create(
            request.Name.Trim(),
            request.Description,
            request.ShortDescription,
            request.Sku.Trim(),
            slug,
            request.Price,
            request.CompareAtPrice,
            request.CostPrice,
            request.Weight,
            request.StockQuantity,
            request.LowStockThreshold,
            request.IsFeatured,
            request.CategoryId);

        await _db.Products.AddAsync(product, ct);
        await _db.SaveChangesAsync(ct);

        return Result<Guid>.Success(product.Id);
    }

    private async Task<string> GenerateUniqueProductSlugAsync(
        string name,
        Guid? currentProductId,
        CancellationToken ct)
    {
        var baseSlug = SlugHelper.FromText(name);
        var slug = baseSlug;
        var counter = 1;

        while (await _db.Products.AnyAsync(p => p.Slug == slug && p.Id != currentProductId, ct))
            slug = $"{baseSlug}-{counter++}";

        return slug;
    }
}
