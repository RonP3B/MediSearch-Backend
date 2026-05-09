using MediSearch.Core.Application.Accounts.DTOs;
using MediSearch.Core.Application.Accounts.Ports;
using MediSearch.Core.Application.Shared.Compensations.Events.ExternalUserDeletion;

namespace MediSearch.Core.Application.Shared.Compensations.CompensableOperations;

internal sealed class RegisterExternalUserCompensableOperation(
    RegisterExternalUserDto registerExternalUserDto,
    IAccountManager accountManager,
    IEventBus eventBus
) : ICompensableOperation<RegisteredExternalUserDto>
{
    public async Task<RegisteredExternalUserDto> ExecuteAsync(CancellationToken cancellationToken)
    {
        var result = await accountManager.RegisterUserAsync(
            registerExternalUserDto,
            cancellationToken
        );

        if (!result.Succeeded)
        {
            throw new ValidationException(result.Errors);
        }

        return result.Value;
    }

    public async Task CompensateAsync(RegisteredExternalUserDto result)
    {
        await eventBus.PublishAsync(new ExternalUserDeletionCompensationEvent(result.Id));
    }
}
