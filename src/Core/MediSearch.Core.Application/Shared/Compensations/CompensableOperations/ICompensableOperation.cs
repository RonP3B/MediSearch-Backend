namespace MediSearch.Core.Application.Shared.Compensations.CompensableOperations;

public interface ICompensableOperation<T>
{
    Task<T> ExecuteAsync(CancellationToken cancellationToken);
    Task CompensateAsync(T result);
}
