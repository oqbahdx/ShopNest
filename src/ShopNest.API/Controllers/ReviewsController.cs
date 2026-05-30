using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopNest.Application.Features.Reviews.Commands.CreateReview;
using ShopNest.Application.Features.Reviews.Commands.DeleteReview;
using ShopNest.Application.Features.Reviews.Commands.UpdateReview;
using ShopNest.Application.Features.Reviews.Queries.CanReviewProduct;
using ShopNest.Application.Features.Reviews.Queries.GetMyReviews;
using ShopNest.Application.Features.Reviews.Queries.GetProductReviews;

namespace ShopNest.API.Controllers;


[Microsoft.AspNetCore.Components.Route("api/v1")]
public sealed class ReviewsController : BaseApiController
{
    // ── Public endpoints ─────────────────────────────────────────────

    /// GET /api/v1/products/{id}/reviews
    [HttpGet("products/{id:guid}/reviews")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductReviews(
        Guid   id,
        [FromQuery] int    page      = 1,
        [FromQuery] int    pageSize  = 10,
        [FromQuery] string sortBy    = "date",
        [FromQuery] string sortOrder = "desc",
        CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new GetProductReviewsQuery(
                id, page, pageSize, sortBy, sortOrder), ct));

    // ── Authenticated customer endpoints ─────────────────────────────

    /// GET /api/v1/products/{id}/reviews/can-review
    [HttpGet("products/{id:guid}/reviews/can-review")]
    [Authorize]
    public async Task<IActionResult> CanReview(
        Guid id, CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new CanReviewProductQuery(id), ct));

    /// POST /api/v1/products/{id}/reviews
    [HttpPost("products/{id:guid}/reviews")]
    [Authorize]
    public async Task<IActionResult> Create(
        Guid id,
        [FromBody] CreateReviewRequest req,
        CancellationToken ct = default)
    {
        var result = await Mediator.Send(
            new CreateReviewCommand(
                id, req.Rating, req.Title, req.Comment), ct);

        return ToResponse(result);
    }

    /// GET /api/v1/my/reviews
    [HttpGet("my/reviews")]
    [Authorize]
    public async Task<IActionResult> GetMyReviews(
        [FromQuery] int page     = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new GetMyReviewsQuery(page, pageSize), ct));

    /// PUT /api/v1/reviews/{id}
    [HttpPut("reviews/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateReviewRequest req,
        CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new UpdateReviewCommand(
                id, req.Rating, req.Title, req.Comment), ct));

    /// DELETE /api/v1/reviews/{id}  (customer — own reviews only)
    [HttpDelete("reviews/{id:guid}")]
    [Authorize]
    public async Task<IActionResult> Delete(
        Guid id, CancellationToken ct = default)
        => ToResponse(await Mediator.Send(
            new DeleteReviewCommand(id, IsAdmin: false), ct));
}

public sealed record CreateReviewRequest(
    int Rating, string Title, string Comment);

public sealed record UpdateReviewRequest(
    int Rating, string Title, string Comment);