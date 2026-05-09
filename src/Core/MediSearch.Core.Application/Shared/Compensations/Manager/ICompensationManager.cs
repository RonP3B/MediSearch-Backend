namespace MediSearch.Core.Application.Shared.Compensations.Manager;

public interface ICompensationManager
{
    Task<T> ExecuteAsync<T>(
        ICompensableOperation<T> compensatedOperation,
        CancellationToken cancellationToken
    );
    Task RollbackAsync();
    void Commit();
}
