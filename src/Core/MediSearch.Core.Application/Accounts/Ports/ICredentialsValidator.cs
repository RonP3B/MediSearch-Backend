namespace MediSearch.Core.Application.Accounts.Ports;

public interface ICredentialsValidator
{
    Task<ServiceResult> ValidateCredentialsAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default
    );
}
