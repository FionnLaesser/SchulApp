extern alias RestApi;

using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using RestApiErrorResponse = RestApi::SchulAppREST.Exceptions.ApiErrorResponse;
using RestGlobalExceptionMiddleware = RestApi::SchulAppREST.Exceptions.GlobalExceptionMiddleware;

namespace SchulApp.Tests;

public class GlobalExceptionMiddlewareTests
{
    [Fact]
    public async Task UnknownException_Returns500WithoutInternalDetails()
    {
        var result = await ExecuteAsync(
            new Exception("Sensitive database password and stack details")
        );

        Assert.Equal(StatusCodes.Status500InternalServerError, result.Context.Response.StatusCode);
        Assert.Equal(StatusCodes.Status500InternalServerError, result.Response.StatusCode);
        Assert.Equal("InternalServerError", result.Response.Error);
        Assert.Equal("An unexpected server error occurred.", result.Response.Message);
        Assert.Equal("exception-handler-test", result.Response.TraceId);
        Assert.DoesNotContain("password", result.RawBody.ToLowerInvariant());
        Assert.DoesNotContain("stack", result.RawBody.ToLowerInvariant());
    }

    [Fact]
    public async Task ArgumentException_Returns400()
    {
        var result = await ExecuteAsync(
            new ArgumentException("Internal validation detail")
        );

        Assert.Equal(StatusCodes.Status400BadRequest, result.Context.Response.StatusCode);
        Assert.Equal(StatusCodes.Status400BadRequest, result.Response.StatusCode);
        Assert.Equal("BadRequest", result.Response.Error);
        Assert.Equal("The request contains invalid data.", result.Response.Message);
        Assert.DoesNotContain("internal validation detail", result.RawBody.ToLowerInvariant());
    }

    [Fact]
    public async Task KeyNotFoundException_Returns404()
    {
        var result = await ExecuteAsync(
            new KeyNotFoundException("Internal entity lookup detail")
        );

        Assert.Equal(StatusCodes.Status404NotFound, result.Context.Response.StatusCode);
        Assert.Equal(StatusCodes.Status404NotFound, result.Response.StatusCode);
        Assert.Equal("NotFound", result.Response.Error);
        Assert.Equal("The requested resource was not found.", result.Response.Message);
    }

    [Fact]
    public async Task UnauthorizedAccessException_Returns403()
    {
        var result = await ExecuteAsync(
            new UnauthorizedAccessException("Internal authorization detail")
        );

        Assert.Equal(StatusCodes.Status403Forbidden, result.Context.Response.StatusCode);
        Assert.Equal(StatusCodes.Status403Forbidden, result.Response.StatusCode);
        Assert.Equal("Forbidden", result.Response.Error);
        Assert.Equal("You are not authorized to perform this action.", result.Response.Message);
    }

    [Fact]
    public async Task InvalidOperationException_Returns409()
    {
        var result = await ExecuteAsync(
            new InvalidOperationException("Internal application state detail")
        );

        Assert.Equal(StatusCodes.Status409Conflict, result.Context.Response.StatusCode);
        Assert.Equal(StatusCodes.Status409Conflict, result.Response.StatusCode);
        Assert.Equal("Conflict", result.Response.Error);
        Assert.Equal(
            "The request conflicts with the current state of the application.",
            result.Response.Message
        );
    }

    private static async Task<HandlerResult> ExecuteAsync(Exception exception)
    {
        var middleware = new RestGlobalExceptionMiddleware(
            _ => Task.FromException(exception),
            NullLogger<RestGlobalExceptionMiddleware>.Instance
        );

        var context = new DefaultHttpContext
        {
            TraceIdentifier = "exception-handler-test"
        };

        context.Request.Method = HttpMethods.Get;
        context.Request.Path = "/api/test";
        context.Response.Body = new MemoryStream();

        await middleware.InvokeAsync(context);

        context.Response.Body.Position = 0;

        using var reader = new StreamReader(context.Response.Body);
        string rawBody = await reader.ReadToEndAsync();

        var response = JsonSerializer.Deserialize<RestApiErrorResponse>(
            rawBody,
            new JsonSerializerOptions(JsonSerializerDefaults.Web)
        );

        Assert.NotNull(response);
        Assert.NotNull(context.Response.ContentType);
        Assert.StartsWith("application/json", context.Response.ContentType);

        return new HandlerResult(context, response, rawBody);
    }

    private sealed record HandlerResult(
        DefaultHttpContext Context,
        RestApiErrorResponse Response,
        string RawBody
    );
}
