using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopNest.Application.Features.Reviews.Commands.ApproveReview;
using ShopNest.Application.Features.Reviews.Commands.DeleteReview;
using ShopNest.Application.Features.Reviews.Commands.RejectReview;
using ShopNest.Application.Features.Reviews.Queries.GetPendingReviews;

namespace ShopNest.API.Controllers;


[Authorize(Roles = "Admin")]
[Route("api/v1/admin/reviews")]
public sealed class AdminReviewsController : BaseApiController
{
    /// GET /api/v1/admin/reviews/pending
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending(
        [FromQuery] int page     = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new GetPendingReviewsQuery(page, pageSize), ct));

    /// POST /api/v1/admin/reviews/{id}/approve
    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(
        Guid id, CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new ApproveReviewCommand(id), ct));

    /// POST /api/v1/admin/reviews/{id}/reject
    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(
        Guid id,
        [FromBody] RejectRequest req,
        CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new RejectReviewCommand(id, req.Note), ct));

    /// DELETE /api/v1/admin/reviews/{id}  (Admin — any review)
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id, CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new DeleteReviewCommand(id, IsAdmin: true), ct));
}

public sealed record RejectRequest(string Note);