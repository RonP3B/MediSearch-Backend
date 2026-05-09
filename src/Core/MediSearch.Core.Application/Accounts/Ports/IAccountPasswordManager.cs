using MediSearch.Core.Application.Accounts.DTOs;

namespace MediSearch.Core.Application.Accounts.Ports;

public interface IAccountPasswordManager
{
    Task<PasswordResetTokenDto> GetPasswordResetTokenAsync(
        string id,
        CancellationToken cancellationToken = default
    );
    Task<ServiceResult> ResetPasswordAsync(
        string id,
        string resetToken,
        string newPassword,
        CancellationToken cancellationToken = default
    );
    Task<ServiceResult> ChangePasswordAsync(
        string id,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default
    );
}
