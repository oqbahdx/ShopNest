using MediatR;
using Microsoft.AspNetCore.Mvc;
using ShopNest.Application.Common.Models;

namespace ShopNest.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private ISender? _mediator;

    // Resolved from DI lazily so derived controllers don't need a constructor
    protected ISender Mediator =>
        _mediator ??= HttpContext.RequestServices
            .GetRequiredService<ISender>();

    /// <summary>
    /// Maps a Result to the appropriate HTTP response.
    /// Controllers stay thin — all status-code logic lives here.
    /// </summary>
    protected IActionResult ToResponse<T>(Result<T> result)
    {
        if (result.IsSuccess)
            return Ok(result.Value);
        return result.ErrorCode switch
        {
            ErrorCodes.NOT_FOUND => NotFound(ProblemDetail(result)),
            ErrorCodes.CONFLICT => Conflict(ProblemDetail(result)),
            ErrorCodes.FORBIDDEN => Forbid(),
            ErrorCodes.VALIDATION_ERROR => UnprocessableEntity(ProblemDetail(result)),
            ErrorCodes.INSUFFICIENT_STOCK => BadRequest(ProblemDetail(result)),
            _ => BadRequest(ProblemDetail(result))
        };
    }

    protected IActionResult ToResponse(Result result)
    {
        if (result.IsSuccess)
            return NoContent();
        return result.ErrorCode switch
        {
            ErrorCodes.NOT_FOUND => NotFound(ProblemDetail(result)),
            ErrorCodes.CONFLICT => Conflict(ProblemDetail(result)),
            ErrorCodes.FORBIDDEN => Forbid(),
            ErrorCodes.VALIDATION_ERROR => UnprocessableEntity(ProblemDetail(result)),
            _ => BadRequest(ProblemDetail(result))
        };
    }

    protected IActionResult CreatedAt<T>(
        string routeName, object routeValues, Result<T> result)
    {
        if (result.IsSuccess)
            return CreatedAtRoute(routeName, routeValues, result.Value);
        return ToResponse(result);
    }

    private static object ProblemDetail(Result result) => new
    {
        title = result.ErrorMessage,
        detail = result.ErrorCode
    };

    private static object ProblemDetail<T>(Result<T> result) => new
    {
        title = result.ErrorMessage,
        detail = result.ErrorCode
    };
}
