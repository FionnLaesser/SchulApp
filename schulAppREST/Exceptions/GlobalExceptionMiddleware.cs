using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SchulAppREST.Exceptions;

public sealed class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
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

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var error = MapException(exception);

        if (error.StatusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Unhandled exception for {Method} {Path}. TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                context.TraceIdentifier
            );
        }
        else
        {
            _logger.LogWarning(
                exception,
                "Handled application exception for {Method} {Path}. StatusCode: {StatusCode}. TraceId: {TraceId}",
                context.Request.Method,
                context.Request.Path,
                error.StatusCode,
                context.TraceIdentifier
            );
        }

        context.Response.Clear();
        context.Response.StatusCode = error.StatusCode;

        var response = new ApiErrorResponse(
            error.StatusCode,
            error.Error,
            error.Message,
            context.TraceIdentifier
        );

        await context.Response.WriteAsJsonAsync(
            response,
            cancellationToken: context.RequestAborted
        );
    }

    private static ExceptionMapping MapException(Exception exception)
    {
        return exception switch
        {
            BadHttpRequestException => new ExceptionMapping(
                StatusCodes.Status400BadRequest,
                HttpStatusCode.BadRequest.ToString(),
                "The request contains invalid data."
            ),

            ArgumentException => new ExceptionMapping(
                StatusCodes.Status400BadRequest,
                HttpStatusCode.BadRequest.ToString(),
                "The request contains invalid data."
            ),

            KeyNotFoundException => new ExceptionMapping(
                StatusCodes.Status404NotFound,
                HttpStatusCode.NotFound.ToString(),
                "The requested resource was not found."
            ),

            UnauthorizedAccessException => new ExceptionMapping(
                StatusCodes.Status403Forbidden,
                HttpStatusCode.Forbidden.ToString(),
                "You are not authorized to perform this action."
            ),

            InvalidOperationException => new ExceptionMapping(
                StatusCodes.Status409Conflict,
                HttpStatusCode.Conflict.ToString(),
                "The request conflicts with the current state of the application."
            ),

            _ => new ExceptionMapping(
                StatusCodes.Status500InternalServerError,
                "InternalServerError",
                "An unexpected server error occurred."
            )
        };
    }

    private sealed record ExceptionMapping(
        int StatusCode,
        string Error,
        string Message
    );
}
