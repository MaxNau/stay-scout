using Grpc.Core;
using Microsoft.AspNetCore.Diagnostics;

namespace StayScout.Gateway.ExceptionHandling;

public sealed class GrpcExceptionHandler(
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not RpcException rpcException)
        {
            return false;
        }

        httpContext.Response.StatusCode = rpcException.StatusCode switch
        {
            StatusCode.InvalidArgument => StatusCodes.Status400BadRequest,
            StatusCode.NotFound => StatusCodes.Status404NotFound,
            StatusCode.DeadlineExceeded => StatusCodes.Status504GatewayTimeout,
            StatusCode.Unavailable => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status502BadGateway
        };

        httpContext.Response.ContentType = "application/problem+json";

        return await problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = new()
                {
                    Status = httpContext.Response.StatusCode,
                    Title = "Search service request failed.",
                    Detail = rpcException.Status.Detail
                }
            });
    }
}
