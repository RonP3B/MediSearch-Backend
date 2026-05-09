namespace MediSearch.Core.Application.Shared.Compensations.Manager;

internal sealed class CompensationManager(ILogger<CompensationManager> logger)
    : ICompensationManager
{
    private readonly Stack<CompensationEntry> _entries = new();
    private readonly ILogger<CompensationManager> _logger = logger;
    private bool _completed;

    public async Task<T> ExecuteAsync<T>(
        ICompensableOperation<T> compensatedOperation,
        CancellationToken cancellationToken
    )
    {
        if (_completed)
        {
            throw new InvalidOperationException(
                "Cannot execute compensated operation after commit."
            );
        }

        var result = await compensatedOperation.ExecuteAsync(cancellationToken);

        _entries.Push(
            new CompensationEntry(
                compensatedOperation.GetType().Name,
                () => compensatedOperation.CompensateAsync(result)
            )
        );

        return result;
    }

    public void Commit()
    {
        _entries.Clear();
        _completed = true;
    }

    public async Task RollbackAsync()
    {
        _completed = true;

        while (_entries.Count > 0)
        {
            var entry = _entries.Pop();

            try
            {
                await entry.CompensateAsync();

                _logger.LogInformation("Compensated operation: {Operation}", entry.OperationName);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to compensate operation: {Operation}",
                    entry.OperationName
                );
            }
        }
    }

    private sealed record CompensationEntry(string OperationName, Func<Task> CompensateAsync);
}
