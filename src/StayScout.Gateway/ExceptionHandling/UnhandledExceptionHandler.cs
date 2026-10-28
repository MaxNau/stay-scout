using Microsoft.AspNetCore.Diagnostics;

namespace StayScout.Gateway.ExceptionHandling;

public sealed partial class UnhandledExceptionHandler(
    ILogger<UnhandledExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        LogUnhandledException(logger, exception);

        httpContext.Response.StatusCode =
            StatusCodes.Status500InternalServerError;

        return await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = new()
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An unexpected error occurred."
                }
            });
    }

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Error,
        Message = "An unhandled exception occurred while processing the request.")]
    private static partial void LogUnhandledException(
        ILogger logger,
        Exception exception);
}
