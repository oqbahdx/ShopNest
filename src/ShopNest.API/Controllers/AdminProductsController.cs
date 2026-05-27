using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopNest.Application.Features.Products.Queries.GetFeaturedProducts;

namespace ShopNest.API.Controllers;

/// <summary>
/// Separate admin controller for admin-only product views.
/// Keeps AdminProductDto fields (CostPrice, StockQuantity, IsDeleted)
/// off the public ProductsController surface area entirely.
/// Route: /api/v1/admin/products
/// </summary>
[ApiController]
[Route("api/v1/admin/products")]
[Authorize(Roles = "Admin")]
public sealed class AdminProductsController : BaseApiController
{
    /// <summary>
    /// GET /api/v1/admin/products
    /// Full admin product list — includes soft-deleted and internal fields.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] bool? isActive = null,
        [FromQuery] bool? isDeleted = null,
        [FromQuery] bool? lowStock = null,
        [FromQuery] string sortBy = "createdAt",
        [FromQuery] string sortOrder = "desc",
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(new GetAdminProductsQuery(
            page, pageSize, search, categoryId,
            isActive, isDeleted, lowStock,
            sortBy, sortOrder), ct);
        return ToResponse(result);
    }
}