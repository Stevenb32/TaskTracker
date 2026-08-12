using Microsoft.AspNetCore.Diagnostics;

namespace TaskTracker.Api.ErrorHandling;

public sealed class BadRequestExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not BadHttpRequestException)
        {
            return false;
        }

        var errors = new Dictionary<string, string[]>
        {
            ["request"] = ["The request payload is invalid."]
        };

        await Results.ValidationProblem(errors, statusCode: StatusCodes.Status400BadRequest, title: "The request payload is invalid.")
            .ExecuteAsync(httpContext);

        return true;
    }
}
