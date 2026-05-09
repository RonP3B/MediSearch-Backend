using MediSearch.Core.Application.Accounts.Constants;
using MediSearch.Core.Application.Accounts.DTOs;
using MediSearch.Core.Application.Accounts.Enums;
using MediSearch.Core.Application.Accounts.Notifications.AccountEmailConfirmed;
using MediSearch.Core.Application.Accounts.Ports;

namespace MediSearch.Core.Application.Accounts.Commands.ConfirmAccountEmail;

public sealed class ConfirmAccountEmailCommandHandler(
    IAccountManager accountManager,
    IEventBus eventBus
) : ICommandHandler<ConfirmAccountEmailCommand, AccountEmailConfirmationDto>
{
    private readonly IAccountManager _accountManager = accountManager;
    private readonly IEventBus _eventBus = eventBus;

    public async Task<AccountEmailConfirmationDto> Handle(
        ConfirmAccountEmailCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var result = await _accountManager.ConfirmEmailAsync(
            cmd.ExternalUserId,
            Uri.UnescapeDataString(cmd.ConfirmationToken),
            cancellationToken
        );

        if (result.HasError(AccountErrorCodes.AccountEmailAlreadyConfirmed))
        {
            return new AccountEmailConfirmationDto
            {
                ActivationStatus = AccountEmailConfirmationStatus.AlreadyConfirmed,
            };
        }

        if (!result.Succeeded)
        {
            return new AccountEmailConfirmationDto
            {
                ActivationStatus = AccountEmailConfirmationStatus.InvalidToken,
            };
        }

        await _eventBus.PublishAsync(
            new AccountEmailConfirmedNotification(
                cmd.ExternalUserId,
                IdempotencyKey: $"{nameof(ConfirmAccountEmailCommand)}-{cmd.ExternalUserId}"
            ),
            cancellationToken
        );

        return new AccountEmailConfirmationDto
        {
            ActivationStatus = AccountEmailConfirmationStatus.Success,
        };
    }
}
