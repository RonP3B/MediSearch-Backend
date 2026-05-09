namespace MediSearch.Infrastructure.MessageBus;

internal sealed class MessageBusOptions
{
    public int PrefetchCount { get; init; }
    public int ConcurrentMessageLimit { get; init; }
    public int RetryCount { get; init; }
    public int RetryInitialIntervalSeconds { get; init; }
    public int RetryMaxIntervalSeconds { get; init; }
    public int RetryIntervalDeltaSeconds { get; init; }
    public int OutboxQueryDelaySeconds { get; init; }
    public int DuplicateDetectionWindowMinutes { get; init; }
}
