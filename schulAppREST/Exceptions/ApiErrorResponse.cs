namespace SchulAppREST.Exceptions;

public sealed record ApiErrorResponse(
    int StatusCode,
    string Error,
    string Message,
    string TraceId
);
