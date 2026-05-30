using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopNest.Application.Features.Notifications.Commands.MarkAllRead;
using ShopNest.Application.Features.Notifications.Commands.MarkNotificationRead;
using ShopNest.Application.Features.Notifications.Queries.GetNotifications;
using ShopNest.Application.Features.Notifications.Queries.GetUnreadCount;

namespace ShopNest.API.Controllers;

[Authorize]
[Route("api/v1/notifications")]
public sealed class NotificationsController : BaseApiController
{
    /// GET /api/v1/notifications?page=1&pageSize=20&isRead=false
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool? isRead = null,
        CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new GetNotificationsQuery(page, pageSize, isRead), ct));

    /// GET /api/v1/notifications/unread-count
    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount(CancellationToken ct = default)
        => ToResponse(await Mediator.Send(new GetUnreadCountQuery(), ct));

    /// PATCH /api/v1/notifications/{id}/read
    [HttpPatch("{id:guid}/read")]
    public async Task<IActionResult> MarkRead(
        Guid id, CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new MarkNotificationReadCommand(id), ct));

    /// PATCH /api/v1/notifications/read-all
    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllRead(CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new MarkAllNotificationsReadCommand(), ct));
}