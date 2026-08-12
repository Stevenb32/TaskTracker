using Microsoft.AspNetCore.Diagnostics;
using TaskTracker.Domain.Exceptions;

namespace TaskTracker.Api.ErrorHandling;

public sealed class DomainValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not DomainValidationException domainException)
        {
            return false;
        }

        var fieldName = string.IsNullOrWhiteSpace(domainException.ParamName) ? "domain" : domainException.ParamName;

        var errors = new Dictionary<string, string[]>
        {
            [fieldName] = [domainException.Message]
        };

        await Results.ValidationProblem(errors, statusCode: StatusCodes.Status400BadRequest, title: "One or more domain validation errors occurred.")
            .ExecuteAsync(httpContext);

        return true;
    }
}