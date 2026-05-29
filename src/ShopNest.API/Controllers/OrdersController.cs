using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopNest.Application.Features.Orders.Commands;
using ShopNest.Application.Features.Orders.Commands.CancelOrder;
using ShopNest.Application.Features.Orders.Commands.RequestReturn;
using ShopNest.Application.Features.Orders.Queries;
using ShopNest.Application.Features.Orders.Queries.GetMyOrders;
using ShopNest.Application.Features.Orders.Queries.GetOrderById;
using ShopNest.Domain.Enums;

namespace ShopNest.API.Controllers;

[Authorize]
[Microsoft.AspNetCore.Components.Route("api/v1/orders")]
public sealed class OrdersController : BaseApiController
{
    /// GET /api/v1/orders
    [HttpGet]
    public async Task<IActionResult> GetMyOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] OrderStatus? status = null,
        CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new GetMyOrdersQuery(page, pageSize, status), ct));

    /// GET /api/v1/orders/{id}
    [HttpGet("{id:guid}", Name = "GetOrderById")]
    public async Task<IActionResult> GetById(
        Guid id, CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new GetOrderByIdQuery(id, IsAdmin: false), ct));

    /// GET /api/v1/orders/number/{orderNumber}
    [HttpGet("number/{orderNumber}")]
    public async Task<IActionResult> GetByNumber(
        string orderNumber, CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new GetOrderByNumberQuery(orderNumber), ct));

    /// POST /api/v1/orders
    [HttpPost]
    public async Task<IActionResult> PlaceOrder(
        [FromBody] PlaceOrderCommand cmd,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(cmd, ct);

        return result.IsSuccess
            ? CreatedAtRoute("GetOrderById",
                new { id = result.Value!.OrderId }, result.Value)
            : ToResponse(result);
    }

    /// POST /api/v1/orders/{id}/cancel
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid id,
        [FromBody] CancelOrderRequest req,
        CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new CancelOrderCommand(id, req.Reason), ct));

    /// POST /api/v1/orders/{id}/return
    [HttpPost("{id:guid}/return")]
    public async Task<IActionResult> RequestReturn(
        Guid id,
        [FromBody] ReturnRequest req,
        CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new RequestReturnCommand(id, req.Reason), ct));
}

public sealed record CancelOrderRequest(string Reason);

public sealed record ReturnRequest(string Reason);