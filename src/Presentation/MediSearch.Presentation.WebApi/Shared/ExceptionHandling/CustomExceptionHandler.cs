using MediSearch.Core.Application.Shared.Exceptions;
using MediSearch.Core.Domain.SharedKernel.Exceptions;
using MediSearch.Infrastructure.Localization.Configuration.Localizers.Error;
using Microsoft.AspNetCore.Diagnostics;

namespace MediSearch.Presentation.WebApi.Shared.ExceptionHandling;

/// <summary>
/// Converts well-known application exceptions into RFC 9110-compliant <see cref="ProblemDetails"/> responses
/// using localized error messages. Unrecognised exceptions return a generic 500 response in non-development
/// environments, and fall through to the default middleware in development.
/// </summary>
internal sealed class CustomExceptionHandler(IWebHostEnvironment environment) : IExceptionHandler
{
    private readonly IWebHostEnvironment _environment = environment;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        var errorLocalizer = httpContext.RequestServices.GetRequiredService<IErrorLocalizer>();

        var (statusCode, problemDetails) = exception switch
        {
            ValidationException ve => (
                StatusCodes.Status400BadRequest,
                new ValidationProblemDetails(
                    ve.Errors.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Select(ec => errorLocalizer[ec]).ToArray()
                    )
                )
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                }
            ),

            BusinessRuleException bre => (
                StatusCodes.Status400BadRequest,
                new ValidationProblemDetails(
                    new Dictionary<string, string[]>
                    {
                        { bre.PropertyName, [errorLocalizer[bre.ErrorCode]] },
                    }
                )
                {
                    Status = StatusCodes.Status400BadRequest,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                }
            ),

            NotFoundException nfe => (
                StatusCodes.Status404NotFound,
                new ProblemDetails
                {
                    Status = StatusCodes.Status404NotFound,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "The specified resource was not found.",
                    Detail = errorLocalizer[nfe.ErrorCode],
                }
            ),

            UnauthorizedException ue => (
                StatusCodes.Status401Unauthorized,
                new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Type = "https://tools.ietf.org/html/rfc7235#section-3.1",
                    Title = "Unauthorized",
                    Detail = errorLocalizer[ue.ErrorCode],
                }
            ),

            ForbiddenAccessException fe => (
                StatusCodes.Status403Forbidden,
                new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                    Title = "Forbidden",
                    Detail = errorLocalizer[fe.ErrorCode],
                }
            ),

            BadHttpRequestException bre => (
                bre.StatusCode,
                new ProblemDetails
                {
                    Status = bre.StatusCode,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Bad Request",
                    Detail = bre.Message,
                }
            ),

            _ => (-1, null),
        };

        if (problemDetails is null)
        {
            if (_environment.IsDevelopment())
            {
                return false;
            }

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(
                new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Title = "An unexpected error occurred.",
                },
                cancellationToken
            );

            return true;
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            problemDetails.GetType(),
            cancellationToken
        );
        return true;
    }
}
