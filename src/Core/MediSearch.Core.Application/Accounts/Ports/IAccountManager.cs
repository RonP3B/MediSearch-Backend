using MediSearch.Core.Application.Accounts.DTOs;

namespace MediSearch.Core.Application.Accounts.Ports;

public interface IAccountManager
{
    Task<ServiceResult<RegisteredExternalUserDto>> RegisterUserAsync(
        RegisterExternalUserDto registerUserDto,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Returns the identifier the external identity provider assigned to the account with
    /// the given username, or <see langword="null"/> when no such account exists there.
    /// </summary>
    Task<string?> FindExternalUserIdByUsernameOrDefaultAsync(
        string username,
        CancellationToken cancellationToken = default
    );

    Task<ServiceResult> DeleteUserAsync(string id, CancellationToken cancellationToken = default);

    Task<AccountEmailConfirmationTokenDto> GetEmailConfirmationTokenAsync(
        string id,
        CancellationToken cancellationToken = default
    );

    Task<ServiceResult> ConfirmEmailAsync(
        string id,
        string activationToken,
        CancellationToken cancellationToken = default
    );
}
