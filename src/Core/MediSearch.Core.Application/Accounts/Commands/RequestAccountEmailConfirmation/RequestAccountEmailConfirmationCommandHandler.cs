using MediSearch.Core.Application.Accounts.Notifications.EmailConfirmationRequested;
using MediSearch.Core.Application.Accounts.Ports;
using MediSearch.Core.Domain.Users;

namespace MediSearch.Core.Application.Accounts.Commands.RequestAccountEmailConfirmation;

public sealed class RequestAccountEmailConfirmationCommandHandler(
    IAccountManager accountManager,
    IAccountQueryService accountQueryService,
    IEventBus eventBus
) : ICommandHandler<RequestAccountEmailConfirmationCommand>
{
    private readonly IAccountManager _accountManager = accountManager;
    private readonly IAccountQueryService _accountQueryService = accountQueryService;
    private readonly IEventBus _eventBus = eventBus;

    public async Task Handle(
        RequestAccountEmailConfirmationCommand cmd,
        CancellationToken cancellationToken
    )
    {
        var externalUserId = await _accountQueryService.GetExternalUserIdByUsernameOrDefaultAsync(
            cmd.Username,
            cancellationToken
        );

        if (externalUserId == null)
        {
            throw NotFoundException.Entity(nameof(User), nameof(User.Username), cmd.Username);
        }

        var confirmationTokenDto = await _accountManager.GetEmailConfirmationTokenAsync(
            externalUserId,
            cancellationToken
        );

        await _eventBus.PublishAsync(
            new EmailConfirmationRequestedNotification(
                externalUserId,
                confirmationTokenDto.ConfirmationToken
            ),
            cancellationToken
        );
    }
}
