using System.Net;
using System.Text.Json;
using Lectio.Api.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Lectio.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = MapExceptionToResponse(exception);

        LogException(exception, statusCode);

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = exception.Message,
            Instance = context.Request.Path
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problemDetails.Status!.Value;

        var payload = JsonSerializer.Serialize(problemDetails);
        await context.Response.WriteAsync(payload);
    }

    private static (HttpStatusCode StatusCode, string Title) MapExceptionToResponse(Exception exception) =>
        exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, "Resource not found"),
            BadRequestException => (HttpStatusCode.BadRequest, "Invalid request"),
            UnauthorizedException => (HttpStatusCode.Unauthorized, "Unauthorized"),
            ConflictException => (HttpStatusCode.Conflict, "Conflict"),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
        };

    private void LogException(Exception exception, HttpStatusCode statusCode)
    {
        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning("Request failed with {StatusCode}: {Message}", (int)statusCode, exception.Message);
        }
    }
}
