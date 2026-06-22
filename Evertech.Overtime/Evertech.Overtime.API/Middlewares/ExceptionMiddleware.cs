using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;
using Evertech.Overtime.Domain.Exceptions;

namespace Evertech.Overtime.API.Middlewares;

[ExcludeFromCodeCoverage]
public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (BusinessException ex)
        {
            logger.LogWarning(ex, "Business rule violation: {Message}", ex.Message);
            await WriteResponseAsync(context, HttpStatusCode.UnprocessableEntity, new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error on {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteResponseAsync(context, HttpStatusCode.InternalServerError, null);
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, object? payload)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        if (payload is not null)
        {
            var json = JsonSerializer.Serialize(payload, JsonOptions);
            await context.Response.WriteAsync(json);
        }
    }
}