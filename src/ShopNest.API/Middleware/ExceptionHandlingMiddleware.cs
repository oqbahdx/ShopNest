using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace ShopNest.API.Middleware;

/// <summary>
/// Global exception handler. Converts unhandled exceptions to RFC 7807
/// Problem Details responses. No try-catch blocks needed in controllers.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate               _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await _next(ctx);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Validation failure on {Path}", ctx.Request.Path);
            await WriteProblemAsync(ctx, 422, "Validation Error",
                string.Join(" | ", ex.Errors.Select(e => e.ErrorMessage)));
        }
        catch (OperationCanceledException)
        {
            // Client disconnected — not an error
            _logger.LogDebug("Request cancelled for {Path}", ctx.Request.Path);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception on {Path}", ctx.Request.Path);
            await WriteProblemAsync(ctx, 500, "Internal Server Error",
                "An unexpected error occurred. Please try again later.");
        }
    }

    private static Task WriteProblemAsync(HttpContext ctx,
        int statusCode, string title, string detail)
    {
        ctx.Response.StatusCode  = statusCode;
        ctx.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title  = title,
            Detail = detail,
            Instance = ctx.Request.Path
        };
        problem.Extensions["traceId"] = ctx.TraceIdentifier;

        return ctx.Response.WriteAsync(
            JsonSerializer.Serialize(problem,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
