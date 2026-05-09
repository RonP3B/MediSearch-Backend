using MediSearch.Core.Application.Accounts.Constants;
using MediSearch.Core.Application.Accounts.Ports;

namespace MediSearch.Core.Application.Shared.Compensations.Events.ExternalUserDeletion;

public sealed class ExternalUserDeletionCompensationEventHandler(IAccountManager accountManager)
    : CompensationEventHandler<ExternalUserDeletionCompensationEvent>
{
    private readonly IAccountManager _accountManager = accountManager;

    public override async Task Handle(
        ExternalUserDeletionCompensationEvent compensationEvent,
        CancellationToken cancellationToken
    )
    {
        var result = await _accountManager.DeleteUserAsync(
            compensationEvent.ExternalUserId,
            cancellationToken
        );

        if (!result.Succeeded && !result.HasError(AccountErrorCodes.AccountDoesNotExist))
        {
            throw new InvalidOperationException(
                $"Failed to delete external user with ID {compensationEvent.ExternalUserId} during compensation."
            );
        }
    }
}
