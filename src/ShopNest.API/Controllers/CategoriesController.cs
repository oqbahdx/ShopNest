using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopNest.API.Models.Products;
using ShopNest.Application.Features.Categories.Commands.CreateCategory;
using ShopNest.Application.Features.Categories.Commands.DeleteCategory;
using ShopNest.Application.Features.Categories.Commands.UpdateCategory;
using ShopNest.Application.Features.GetCategories.Queries.GetCategories;
using ShopNest.Application.Features.GetCategories.Queries.GetCategoryBySlug;
using ShopNest.Application.Features.GetCategories.Queries.GetProductsByCategory;
using ShopNest.Application.Features.Products.DTOs;

namespace ShopNest.API.Controllers;

[ApiController]
[Route("api/v1/categories")]
public sealed class CategoriesController : BaseApiController
{
    // ──────────────────────────────────────────────
    // PUBLIC ENDPOINTS
    // ──────────────────────────────────────────────
    /// <summary>
    /// GET /api/v1/categories
    /// Full category tree with sub-categories and product counts.
    /// Response is cached 60 min in GetCategoriesQueryHandler.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
    {
        var result = await Mediator.Send(new GetCategoriesQuery(), ct);
        return ToResponse(result);
    }

    /// <summary>
    /// GET /api/v1/categories/slug/{slug}
    /// NOTE: declared before /{id} to prevent route ambiguity.
    /// </summary>
    [HttpGet("slug/{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBySlug(
        string slug, CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new GetCategoryBySlugQuery(slug), ct);
        return ToResponse(result);
    }

    /// <summary>
    /// GET /api/v1/categories/{id}
    /// </summary>
    [HttpGet("{id:guid}", Name = "GetCategoryById")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(
        Guid id, CancellationToken ct = default)
    {
        // Re-use GetCategoryBySlugQuery pattern but by ID.
        // A dedicated GetCategoryByIdQuery can be added later if slug lookup
        // causes any ambiguity; for now we resolve via the tree cache.
        var treeResult = await Mediator.Send(new GetCategoriesQuery(), ct);
        if (!treeResult.IsSuccess)
            return ToResponse(treeResult);
        var category = FindInTree(treeResult.Value!, id);
        if (category is null)
            return NotFound(new { title = "Category not found.", detail = "NOT_FOUND" });
        return Ok(category);
    }

    /// <summary>
    /// GET /api/v1/categories/{id}/products
    /// Paginated products in this category + all sub-categories.
    /// </summary>
    [HttpGet("{id:guid}/products")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProducts(
        Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sortBy = "createdAt",
        [FromQuery] string? sortOrder = "desc",
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new GetProductsByCategoryQuery(id, page, pageSize, sortBy, sortOrder), ct);
        return ToResponse(result);
    }

    // ──────────────────────────────────────────────
    // ADMIN ENDPOINTS
    // ──────────────────────────────────────────────
    /// <summary>
    /// POST /api/v1/categories
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryRequest req,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(new CreateCategoryCommand(
            req.Name, req.Description, req.ImageUrl,
            req.DisplayOrder, req.ParentCategoryId), ct);
        return CreatedAt("GetCategoryById", new { id = result.Value }, result);
    }

    /// <summary>
    /// PUT /api/v1/categories/{id}
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCategoryRequest req,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(new UpdateCategoryCommand(
            id, req.Name, req.Description, req.ImageUrl,
            req.DisplayOrder, req.ParentCategoryId), ct);
        return ToResponse(result);
    }

    /// <summary>
    /// DELETE /api/v1/categories/{id}
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(
        Guid id, CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new DeleteCategoryCommand(id), ct);
        return ToResponse(result);
    }

    // ──────────────────────────────────────────────
    // PRIVATE HELPERS
    // ──────────────────────────────────────────────
    private static CategoryDto? FindInTree(
        IReadOnlyList<CategoryDto> nodes,
        Guid id)
    {
        foreach (var node in nodes)
        {
            if (node.Id == id) return node;
            var found = FindInTree(node.SubCategories, id);
            if (found is not null) return found;
        }

        return null;
    }
}
