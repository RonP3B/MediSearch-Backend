using MediatR.Pipeline;

namespace MediSearch.Core.Application.Shared.Behaviors;

internal sealed class LoggingBehavior<TRequest>(ILogger<TRequest> logger, ICurrentUser currentUser)
    : IRequestPreProcessor<TRequest>
    where TRequest : notnull
{
    private readonly ILogger _logger = logger;
    private readonly ICurrentUser _currentUser = currentUser;

    public Task Process(TRequest request, CancellationToken cancellationToken)
    {
        string requestName = typeof(TRequest).Name;

        Guid userId = _currentUser.Id ?? Guid.Empty;

        string userInfo = userId != Guid.Empty ? "(ID: {userId})" : "No authenticated user";

        _logger.LogInformation(
            "[MediSearch Application] Handling Request: '{RequestName}' | User: {UserInfo}",
            requestName,
            userInfo
        );

        return Task.CompletedTask;
    }
}
