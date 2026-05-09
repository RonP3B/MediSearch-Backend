using MediSearch.Core.Application.Accounts.Constants;
using MediSearch.Core.Application.Accounts.Notifications.PasswordResetSuccess;
using MediSearch.Core.Application.Accounts.Ports;

namespace MediSearch.Core.Application.Accounts.Commands.ResetPassword;

public sealed class ResetPasswordCommandHandler(
    IAccountPasswordManager accountPasswordManager,
    IEventBus eventBus
) : ICommandHandler<ResetPasswordCommand>
{
    private readonly IAccountPasswordManager _accountPasswordManager = accountPasswordManager;
    private readonly IEventBus _eventBus = eventBus;

    public async Task Handle(ResetPasswordCommand cmd, CancellationToken cancellationToken)
    {
        var result = await _accountPasswordManager.ResetPasswordAsync(
            cmd.ExternalUserId,
            Uri.UnescapeDataString(cmd.ResetToken),
            cmd.NewPassword,
            cancellationToken
        );

        if (result.HasError(AccountErrorCodes.AccountDoesNotExist))
        {
            throw new NotFoundException();
        }

        if (!result.Succeeded)
        {
            throw new ValidationException(result.Errors);
        }

        await _eventBus.PublishAsync(
            new PasswordResetSuccessNotification(cmd.ExternalUserId),
            cancellationToken
        );
    }
}
