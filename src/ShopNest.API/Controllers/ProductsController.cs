using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopNest.API.Models.Products;
using ShopNest.Application.Features.Products.Commands.CreateProduct;
using ShopNest.Application.Features.Products.Commands.DeleteProduct;
using ShopNest.Application.Features.Products.Commands.DeleteProductImage;
using ShopNest.Application.Features.Products.Commands.SetPrimaryImage;
using ShopNest.Application.Features.Products.Commands.UpdateProduct;
using ShopNest.Application.Features.Products.Commands.UpdateStock;
using ShopNest.Application.Features.Products.Commands.UploadProductImages;
using ShopNest.Application.Features.Products.Queries.GetFeaturedProducts;
using ShopNest.Application.Features.Products.Queries.GetProductBySlug;
using ShopNest.Application.Features.Products.Queries.GetProducts;
using ShopNest.Application.Features.Products.Queries.SearchProducts;

namespace ShopNest.API.Controllers;

[ApiController]
[Route("api/v1/products")]
public sealed class ProductsController : BaseApiController
{
    // ──────────────────────────────────────────────
    // PUBLIC ENDPOINTS
    // ──────────────────────────────────────────────
    /// <summary>
    /// GET /api/v1/products
    /// Paginated product list with full filter + sort support.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] bool? inStock = null,
        [FromQuery] bool? featured = null,
        [FromQuery] decimal? minRating = null,
        [FromQuery] string sortBy = "createdAt",
        [FromQuery] string sortOrder = "desc",
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(new GetProductsQuery(
            page, pageSize, search, categoryId,
            minPrice, maxPrice, inStock, featured,
            minRating, sortBy, sortOrder), ct);
        return ToResponse(result);
    }

    /// <summary>
    /// GET /api/v1/products/featured
    /// NOTE: This route must be declared BEFORE /{id} to avoid route conflict.
    /// </summary>
    [HttpGet("featured")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFeatured(
        [FromQuery] int top = 10,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new GetFeaturedProductsQuery(top), ct);
        return ToResponse(result);
    }

    /// <summary>
    /// GET /api/v1/products/search?q=...
    /// Dedicated full-text search endpoint.
    /// </summary>
    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search(
        [FromQuery] string q,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new SearchProductsQuery(q, page, pageSize), ct);
        return ToResponse(result);
    }

    /// <summary>
    /// GET /api/v1/products/slug/{slug}
    /// SEO-friendly product lookup.
    /// </summary>
    [HttpGet("slug/{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySlug(
        string slug, CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new GetProductBySlugQuery(slug), ct);
        return ToResponse(result);
    }

    /// <summary>
    /// GET /api/v1/products/{id}
    /// </summary>
    [HttpGet("{id:guid}", Name = "GetProductById")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(
        Guid id, CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new GetProductByIdQuery(id), ct);
        return ToResponse(result);
    }

    // ──────────────────────────────────────────────
    // ADMIN ENDPOINTS
    // ──────────────────────────────────────────────
    /// <summary>
    /// POST /api/v1/products
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        [FromBody] CreateProductRequest req,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(new CreateProductCommand(
            req.Name, req.Description, req.ShortDescription,
            req.Sku, req.Price, req.CompareAtPrice, req.CostPrice,
            req.Weight, req.StockQuantity, req.LowStockThreshold,
            req.IsFeatured, req.CategoryId), ct);
        return CreatedAt("GetProductById", new { id = result.Value }, result);
    }

    /// <summary>
    /// PUT /api/v1/products/{id}
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateProductRequest req,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(new UpdateProductCommand(
            id, req.Name, req.Description, req.ShortDescription,
            req.Sku, req.Price, req.CompareAtPrice, req.CostPrice,
            req.Weight, req.LowStockThreshold,
            req.IsFeatured, req.IsActive, req.CategoryId), ct);
        return ToResponse(result);
    }

    /// <summary>
    /// PATCH /api/v1/products/{id}/stock
    /// </summary>
    [HttpPatch("{id:guid}/stock")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStock(
        Guid id,
        [FromBody] UpdateStockRequest req,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new UpdateStockCommand(id, req.NewQuantity, req.Reason), ct);
        return ToResponse(result);
    }

    /// <summary>
    /// DELETE /api/v1/products/{id}
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(
        Guid id, CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new DeleteProductCommand(id), ct);
        return ToResponse(result);
    }

    // ──────────────────────────────────────────────
    // IMAGE ENDPOINTS (Admin)
    // ──────────────────────────────────────────────
    /// <summary>
    /// POST /api/v1/products/{id}/images
    /// Multipart/form-data upload — up to 10 images per request.
    /// </summary>
    [HttpPost("{id:guid}/images")]
    [Authorize(Roles = "Admin")]
    [RequestSizeLimit(100 * 1024 * 1024)] // 100 MB total per request
    public async Task<IActionResult> UploadImages(
        Guid id,
        [FromForm] IFormFileCollection images,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new UploadProductImagesCommand(id, images.ToList()), ct);
        return ToResponse(result);
    }

    /// <summary>
    /// DELETE /api/v1/products/{id}/images/{imageId}
    /// </summary>
    [HttpDelete("{id:guid}/images/{imageId:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteImage(
        Guid id, Guid imageId, CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new DeleteProductImageCommand(id, imageId), ct);
        return ToResponse(result);
    }

    /// <summary>
    /// PATCH /api/v1/products/{id}/images/{imageId}/primary
    /// </summary>
    [HttpPatch("{id:guid}/images/{imageId:guid}/primary")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetPrimaryImage(
        Guid id, Guid imageId, CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new SetPrimaryImageCommand(id, imageId), ct);
        return ToResponse(result);
    }
}
