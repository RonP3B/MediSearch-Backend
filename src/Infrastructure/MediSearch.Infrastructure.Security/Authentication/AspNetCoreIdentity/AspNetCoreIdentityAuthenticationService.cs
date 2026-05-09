using MediSearch.Core.Application.Accounts.Constants;
using MediSearch.Core.Application.Accounts.DTOs;
using MediSearch.Core.Application.Accounts.Ports;
using MediSearch.Core.Application.Shared.Results;
using MediSearch.Infrastructure.Security.Authentication.Constants;
using Microsoft.AspNetCore.Identity;

namespace MediSearch.Infrastructure.Security.Authentication.AspNetCoreIdentity;

internal sealed class AspNetCoreIdentityAuthenticationService(UserManager<IdentityUser> userManager)
    : ICredentialsValidator,
        IAccountManager,
        IAccountPasswordManager
{
    private readonly UserManager<IdentityUser> _userManager = userManager;

    public async Task<ServiceResult> ConfirmEmailAsync(
        string id,
        string activationToken,
        CancellationToken cancellationToken = default
    )
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
        {
            return ServiceResult.Failure(
                AuthenticationErrorKeys.Account,
                AccountErrorCodes.AccountDoesNotExist
            );
        }

        if (user.EmailConfirmed)
        {
            return ServiceResult.Failure(
                AuthenticationErrorKeys.Account,
                AccountErrorCodes.AccountEmailAlreadyConfirmed
            );
        }

        IdentityResult identityResult = await _userManager.ConfirmEmailAsync(user, activationToken);

        if (!identityResult.Succeeded)
        {
            return ServiceResult.Failure(
                AuthenticationErrorKeys.Token,
                AccountErrorCodes.InvalidToken
            );
        }

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> ChangePasswordAsync(
        string id,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default
    )
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
        {
            return ServiceResult.Failure(
                AuthenticationErrorKeys.Account,
                AccountErrorCodes.AccountDoesNotExist
            );
        }

        IdentityResult identityResult = await _userManager.ChangePasswordAsync(
            user,
            currentPassword,
            newPassword
        );

        return identityResult.ToServiceResult();
    }

    public async Task<ServiceResult> DeleteUserAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
        {
            return ServiceResult.Failure(
                AuthenticationErrorKeys.Account,
                AccountErrorCodes.AccountDoesNotExist
            );
        }

        IdentityResult identityResult = await _userManager.DeleteAsync(user);

        return identityResult.ToServiceResult();
    }

    public async Task<AccountEmailConfirmationTokenDto> GetEmailConfirmationTokenAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        var user = await _userManager.FindByIdAsync(id);

        Guard.Against.NotFound(id, user);

        return new AccountEmailConfirmationTokenDto
        {
            ConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user),
        };
    }

    public async Task<PasswordResetTokenDto> GetPasswordResetTokenAsync(
        string id,
        CancellationToken cancellationToken = default
    )
    {
        var user = await _userManager.FindByIdAsync(id);

        Guard.Against.NotFound(id, user);

        return new PasswordResetTokenDto
        {
            ResetToken = await _userManager.GeneratePasswordResetTokenAsync(user),
        };
    }

    public async Task<ServiceResult<RegisteredExternalUserDto>> RegisterUserAsync(
        RegisterExternalUserDto registerUserDto,
        CancellationToken cancellationToken = default
    )
    {
        IdentityUser user = new()
        {
            UserName = registerUserDto.Username,
            Email = registerUserDto.Email,
            PhoneNumber = registerUserDto.PhoneNumber,
            EmailConfirmed = registerUserDto.IsActive,
        };

        IdentityResult identityResult = await _userManager.CreateAsync(
            user,
            registerUserDto.Password
        );

        return identityResult.ToServiceResult(new RegisteredExternalUserDto { Id = user.Id });
    }

    public async Task<ServiceResult> ResetPasswordAsync(
        string id,
        string resetToken,
        string newPassword,
        CancellationToken cancellationToken = default
    )
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
        {
            return ServiceResult.Failure(
                AuthenticationErrorKeys.Account,
                AccountErrorCodes.AccountDoesNotExist
            );
        }

        IdentityResult identityResult = await _userManager.ResetPasswordAsync(
            user,
            resetToken,
            newPassword
        );

        return identityResult.ToServiceResult();
    }

    public async Task<ServiceResult> ValidateCredentialsAsync(
        string username,
        string password,
        CancellationToken cancellationToken = default
    )
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
        {
            return ServiceResult.Failure(
                AuthenticationErrorKeys.Credentials,
                AccountErrorCodes.InvalidCredentials
            );
        }

        if (!user.EmailConfirmed)
        {
            return ServiceResult.Failure(
                AuthenticationErrorKeys.Account,
                AccountErrorCodes.AccountEmailNotConfirmed
            );
        }

        return ServiceResult.Success();
    }
}
