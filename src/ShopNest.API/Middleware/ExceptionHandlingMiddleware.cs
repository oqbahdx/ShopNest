using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ShopNest.API.Middleware;

public sealed class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (ValidationException ex)
        {
            logger.LogWarning("Validation failure on {Path}", ctx.Request.Path);
            await WriteProblemAsync(
                ctx, 422, "Validation Error",
                string.Join(" | ", ex.Errors.Select(e => e.ErrorMessage)));
        }
        catch (OperationCanceledException)
        {
            logger.LogDebug("Request cancelled for {Path}", ctx.Request.Path);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception on {Path}", ctx.Request.Path);
            await WriteProblemAsync(
                ctx, 500, "Internal Server Error",
                "An unexpected error occurred. Please try again later.");
        }
    }

    private static Task WriteProblemAsync(
        HttpContext ctx, int statusCode, string title, string detail)
    {
        ctx.Response.StatusCode  = statusCode;
        ctx.Response.ContentType = "application/problem+json";

        var problem = new ProblemDetails
        {
            Status   = statusCode,
            Title    = title,
            Detail   = detail,
            Instance = ctx.Request.Path
        };
        problem.Extensions["traceId"] = ctx.TraceIdentifier;

        return ctx.Response.WriteAsync(
            JsonSerializer.Serialize(problem, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));
    }
}
