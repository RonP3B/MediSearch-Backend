using MediSearch.Core.Application.Accounts.Constants;
using MediSearch.Core.Application.Accounts.Notifications.PasswordChanged;
using MediSearch.Core.Application.Accounts.Ports;

namespace MediSearch.Core.Application.Accounts.Commands.ChangePassword;

public sealed class ChangePasswordCommandHandler(
    IAccountPasswordManager accountPasswordManager,
    ICurrentUser currentUser,
    IEventBus eventBus
) : ICommandHandler<ChangePasswordCommand>
{
    private readonly IAccountPasswordManager _accountPasswordManager = accountPasswordManager;
    private readonly ICurrentUser _currentUser = currentUser;
    private readonly IEventBus _eventBus = eventBus;

    public async Task Handle(ChangePasswordCommand cmd, CancellationToken cancellationToken)
    {
        string externalUserId = _currentUser.GetAuthenticatedExternalUserId();

        var result = await _accountPasswordManager.ChangePasswordAsync(
            externalUserId,
            cmd.CurrentPassword,
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
            new PasswordChangedNotification(externalUserId),
            cancellationToken
        );
    }
}
