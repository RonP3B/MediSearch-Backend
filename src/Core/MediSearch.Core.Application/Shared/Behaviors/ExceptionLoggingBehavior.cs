using MediatR;
using MediSearch.Core.Domain.SharedKernel.Exceptions;

namespace MediSearch.Core.Application.Shared.Behaviors;

internal sealed class ExceptionLoggingBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return await next(cancellationToken);
        }
        catch (Exception ex) when (IsExpected(ex))
        {
            _logger.LogInformation(
                "[MediSearch Application Request]: Handled exception for {Name}",
                typeof(TRequest).Name
            );

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "[MediSearch Application Request]: Unhandled Exception for Request {Name} {@Request}",
                typeof(TRequest).Name,
                request
            );

            throw;
        }
    }

    private static bool IsExpected(Exception ex)
    {
        return ex
            is ValidationException
                or BusinessRuleException
                or NotFoundException
                or UnauthorizedException
                or ForbiddenAccessException;
    }
}
