using FarmHelm.Application.Common.Exceptions;
using FarmHelm.Domain.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FarmHelm.Api.ErrorHandling;

public sealed class ApiExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, title, detail) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Resource not found.", exception.Message),
            ApplicationValidationException => (StatusCodes.Status400BadRequest, "Validation failed.", exception.Message),
            ConflictException => (StatusCodes.Status409Conflict, "Conflict.", exception.Message),
            DomainException => (StatusCodes.Status400BadRequest, "Business rule violated.", exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "Unexpected error.", "An unexpected error occurred.")
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "An unhandled exception occurred while processing the request.");
        }

        httpContext.Response.StatusCode = status;
        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = detail
            },
            Exception = exception
        });
    }
}
