using System.Globalization;
using System.Net;
using MediSearch.Core.Application.Accounts.Constants;
using MediSearch.Core.Application.Accounts.DTOs;
using MediSearch.Core.Application.Accounts.Ports;
using MediSearch.Core.Application.Shared.Results;
using MediSearch.Infrastructure.Security.Authentication.AccountTokens;
using MediSearch.Infrastructure.Security.Authentication.Constants;
using MediSearch.Infrastructure.Security.Authentication.Keycloak.Contracts;

namespace MediSearch.Infrastructure.Security.Authentication.Keycloak;

/// <summary>
/// Keycloak-backed replacement for the ASP.NET Core Identity adapter. It implements the
/// same three application ports, so nothing above the infrastructure layer changes: the
/// API still issues its own JWTs and still owns the account emails. Keycloak only stores
/// the accounts and their credentials.
/// </summary>
internal sealed class KeycloakAccountService(
    KeycloakAdminApiClient adminApiClient,
    KeycloakTokenClient tokenClient,
    AccountActionTokenService accountActionTokenService
) : ICredentialsValidator, IAccountManager, IAccountPasswordManager
{
    private readonly KeycloakAdminApiClient _adminApiClient = adminApiClient;
    private readonly KeycloakTokenClient _tokenClient = tokenClient;
    private readonly AccountActionTokenService _accountActionTokenService =
        accountActionTokenService;

    public async Task<ServiceResult<RegisteredExternalUserDto>> RegisterUserAsync(
        RegisterExternalUserDto registerUserDto,
        CancellationToken cancellationToken = default
    )
    {
        KeycloakUserUpsertRequest request = new()
        {
            Username = registerUserDto.Username,
            Email = registerUserDto.Email,

            // The account is always enabled: the password grant has to work before the
            // email is confirmed so that ValidateCredentialsAsync can tell "wrong password"
            // and "email not confirmed" apart, exactly like IdentityUser.EmailConfirmed did.
            Enabled = true,
            EmailVerified = registerUserDto.IsActive,
            Credentials =
            [
                new KeycloakCredentialRepresentation
                {
                    Type = KeycloakConstants.PasswordCredentialType,
                    Value = registerUserDto.Password,
                    Temporary = false,
                },
            ],
        };

        KeycloakAdminResponse response = await _adminApiClient.CreateUserAsync(
            request,
            cancellationToken
        );

        if (!response.IsSuccess)
        {
            return ServiceResult<RegisteredExternalUserDto>.Failure(
                KeycloakErrorTranslator.Translate(response)
            );
        }

        string externalUserId = Guard.Against.NullOrWhiteSpace(
            response.CreatedResourceId,
            message: "Keycloak created the account but did not return its identifier."
        );

        return ServiceResult<RegisteredExternalUserDto>.Success(
            new RegisteredExternalUserDto { Id = externalUserId }
        );
    }

    public async Task<string?> FindExternalUserIdByUsernameOrDefaultAsync(
        string username,
        CancellationToken cancellationToken = default
    )
    {
        KeycloakUserRepresentation? user = await _adminApiClient.FindUserByUsernameOrDefaultAsync(
            username,
            cancellationToken
        );

        return user?.Id;
    }

    public async Task<ServiceResult> DeleteUserAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        KeycloakAdminResponse response = await _adminApiClient.DeleteUserAsync(
            id,
            cancellationToken
        );

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return AccountDoesNotExist();
        }

        return response.IsSuccess
            ? ServiceResult.Success()
            : ServiceResult.Failure(KeycloakErrorTranslator.Translate(response));
    }

    public async Task<AccountEmailConfirmationTokenDto> GetEmailConfirmationTokenAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        KeycloakUserRepresentation? user = await _adminApiClient.FindUserByIdOrDefaultAsync(
            id,
            cancellationToken
        );

        Guard.Against.NotFound(id, user);

        return new AccountEmailConfirmationTokenDto
        {
            ConfirmationToken = _accountActionTokenService.CreateEmailConfirmationToken(id),
        };
    }

    public async Task<ServiceResult> ConfirmEmailAsync(
        string id,
        string activationToken,
        CancellationToken cancellationToken = default
    )
    {
        KeycloakUserRepresentation? user = await _adminApiClient.FindUserByIdOrDefaultAsync(
            id,
            cancellationToken
        );

        if (user is null)
        {
            return AccountDoesNotExist();
        }

        if (user.EmailVerified)
        {
            return ServiceResult.Failure(
                AuthenticationErrorKeys.Account,
                AccountErrorCodes.AccountEmailAlreadyConfirmed
            );
        }

        if (!_accountActionTokenService.IsEmailConfirmationTokenValid(activationToken, id))
        {
            return InvalidToken();
        }

        // Keycloak stopped supporting partial user updates when the declarative user
        // profile became mandatory: fields missing from the representation can be wiped
        // instead of left alone. So the account is echoed back in full, with only
        // emailVerified changed.
        KeycloakAdminResponse response = await _adminApiClient.UpdateUserAsync(
            id,
            new KeycloakUserUpsertRequest
            {
                Username = user.Username,
                Email = user.Email,
                Enabled = user.Enabled,
                EmailVerified = true,
            },
            cancellationToken
        );

        return response.IsSuccess
            ? ServiceResult.Success()
            : ServiceResult.Failure(KeycloakErrorTranslator.Translate(response));
    }

    public async Task<PasswordResetTokenDto> GetPasswordResetTokenAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        KeycloakUserRepresentation? user = await _adminApiClient.FindUserByIdOrDefaultAsync(
            id,
            cancellationToken
        );

        Guard.Against.NotFound(id, user);

        string stamp = await GetPasswordStampAsync(id, cancellationToken);

        return new PasswordResetTokenDto
        {
            ResetToken = _accountActionTokenService.CreatePasswordResetToken(id, stamp),
        };
    }

    public async Task<ServiceResult> ResetPasswordAsync(
        string id,
        string resetToken,
        string newPassword,
        CancellationToken cancellationToken = default
    )
    {
        KeycloakUserRepresentation? user = await _adminApiClient.FindUserByIdOrDefaultAsync(
            id,
            cancellationToken
        );

        if (user is null)
        {
            return AccountDoesNotExist();
        }

        string stamp = await GetPasswordStampAsync(id, cancellationToken);

        if (!_accountActionTokenService.IsPasswordResetTokenValid(resetToken, id, stamp))
        {
            return InvalidToken();
        }

        KeycloakAdminResponse response = await _adminApiClient.ResetPasswordAsync(
            id,
            newPassword,
            cancellationToken
        );

        return response.IsSuccess
            ? ServiceResult.Success()
            : ServiceResult.Failure(KeycloakErrorTranslator.Translate(response));
    }

    public async Task<ServiceResult> ChangePasswordAsync(
        string id,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default
    )
    {
        KeycloakUserRepresentation? user = await _adminApiClient.FindUserByIdOrDefaultAsync(
            id,
            cancellationToken
        );

        if (user is null || string.IsNullOrWhiteSpace(user.Username))
        {
            return AccountDoesNotExist();
        }

        // Keycloak has no "change password with the current one" endpoint. The current
        // password is therefore verified through the password grant before the new one is set.
        bool isCurrentPasswordValid = await _tokenClient.IsPasswordValidAsync(
            user.Username,
            currentPassword,
            cancellationToken
        );

        if (!isCurrentPasswordValid)
        {
            return ServiceResult.Failure(
                AuthenticationErrorKeys.Password,
                AccountErrorCodes.PasswordMismatch
            );
        }

        KeycloakAdminResponse response = await _adminApiClient.ResetPasswordAsync(
            id,
            newPassword,
            cancellationToken
        );

        return response.IsSuccess
            ? ServiceResult.Success()
            : ServiceResult.Failure(KeycloakErrorTranslator.Translate(response));
    }

    public async Task<ServiceResult> ValidateCredentialsAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default
    )
    {
        KeycloakUserRepresentation? user = await _adminApiClient.FindUserByUsernameOrDefaultAsync(
            username,
            cancellationToken
        );

        if (user is null || !user.Enabled || string.IsNullOrWhiteSpace(user.Username))
        {
            return InvalidCredentials();
        }

        bool isPasswordValid = await _tokenClient.IsPasswordValidAsync(
            user.Username,
            password,
            cancellationToken
        );

        if (!isPasswordValid)
        {
            return InvalidCredentials();
        }

        if (!user.EmailVerified)
        {
            return ServiceResult.Failure(
                AuthenticationErrorKeys.Account,
                AccountErrorCodes.AccountEmailNotConfirmed
            );
        }

        return ServiceResult.Success();
    }

    /// <summary>
    /// The stamp embedded in password reset links. Keycloak never exposes the password
    /// hash, but it does expose when the password credential was created, and that value
    /// changes on every reset. Embedding it makes a reset link single use.
    /// </summary>
    private async Task<string> GetPasswordStampAsync(
        string id,
        CancellationToken cancellationToken
    )
    {
        long? createdDate = await _adminApiClient.GetPasswordCredentialTimestampOrDefaultAsync(
            id,
            cancellationToken
        );

        return createdDate?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
    }

    private static ServiceResult AccountDoesNotExist()
    {
        return ServiceResult.Failure(
            AuthenticationErrorKeys.Account,
            AccountErrorCodes.AccountDoesNotExist
        );
    }

    private static ServiceResult InvalidCredentials()
    {
        return ServiceResult.Failure(
            AuthenticationErrorKeys.Credentials,
            AccountErrorCodes.InvalidCredentials
        );
    }

    private static ServiceResult InvalidToken()
    {
        return ServiceResult.Failure(
            AuthenticationErrorKeys.Token,
            AccountErrorCodes.InvalidToken
        );
    }
}
