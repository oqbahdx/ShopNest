using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopNest.Application.Features.Orders.Commands.UpdateOrderStatus;
using ShopNest.Application.Features.Orders.Queries.GetAdminOrders;
using ShopNest.Application.Features.Orders.Queries.GetOrderById;
using ShopNest.Application.Features.Orders.Queries.GetOrderSummary;
using ShopNest.Domain.Enums;

namespace ShopNest.API.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/v1/admin/orders")]
public sealed class AdminOrdersController : BaseApiController
{
    /// GET /api/v1/admin/orders
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] OrderStatus? status = null,
        [FromQuery] Guid? userId = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] decimal? minAmount = null,
        [FromQuery] decimal? maxAmount = null,
        [FromQuery] string sortBy = "createdAt",
        [FromQuery] string sortOrder = "desc",
        CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new GetAdminOrdersQuery(page, pageSize, status, userId,
                from, to, minAmount, maxAmount, sortBy, sortOrder), ct));

    /// GET /api/v1/admin/orders/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id, CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new GetOrderByIdQuery(id, IsAdmin: true), ct));

    /// GET /api/v1/admin/orders/summary
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary(CancellationToken ct = default)
        => ToResponse(await Mediator.Send(new GetOrderSummaryQuery(), ct));

    /// PATCH /api/v1/admin/orders/{id}/status
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateStatusRequest req,
        CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new UpdateOrderStatusCommand(id, req.Status, req.TrackingNumber), ct));
}

public sealed record UpdateStatusRequest(
    OrderStatus Status,
    string? TrackingNumber = null
);