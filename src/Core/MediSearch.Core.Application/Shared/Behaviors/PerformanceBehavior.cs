using System.Diagnostics;
using MediatR;

namespace MediSearch.Core.Application.Shared.Behaviors;

internal sealed class PerformanceBehavior<TRequest, TResponse>(
    ILogger<TRequest> logger,
    ICurrentUser currentUser
) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly Stopwatch _timer = new();
    private readonly ILogger<TRequest> _logger = logger;
    private readonly ICurrentUser _currentUser = currentUser;
    private const int WarningThresholdMs = 500;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        _timer.Start();

        TResponse response = await next(cancellationToken);

        _timer.Stop();

        long elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > WarningThresholdMs)
        {
            string requestName = typeof(TRequest).Name;

            Guid userId = _currentUser.Id ?? Guid.Empty;

            string userInfo = userId != Guid.Empty ? $" (ID: {userId})" : "No authenticated user";

            _logger.LogWarning(
                "[Performance Warning] Request '{RequestName}' took {ElapsedMilliseconds}ms to complete. "
                    + "Threshold is {Threshold}ms. User: {UserInfo}",
                requestName,
                elapsedMilliseconds,
                WarningThresholdMs,
                userInfo
            );
        }

        return response;
    }
}
