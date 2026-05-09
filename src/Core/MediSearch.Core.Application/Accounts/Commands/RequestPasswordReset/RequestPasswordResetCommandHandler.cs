using MediSearch.Core.Application.Accounts.Notifications.PasswordResetRequested;
using MediSearch.Core.Application.Accounts.Ports;
using MediSearch.Core.Domain.Users;

namespace MediSearch.Core.Application.Accounts.Commands.RequestPasswordReset;

public sealed class RequestPasswordResetCommandHandler(
    IAccountPasswordManager accountPasswordManager,
    IAccountQueryService accountQueryService,
    IEventBus eventBus
) : ICommandHandler<RequestPasswordResetCommand>
{
    private readonly IAccountPasswordManager _accountPasswordManager = accountPasswordManager;
    private readonly IAccountQueryService _accountQueryService = accountQueryService;
    private readonly IEventBus _eventBus = eventBus;

    public async Task Handle(RequestPasswordResetCommand cmd, CancellationToken cancellationToken)
    {
        var externalUserId = await _accountQueryService.GetExternalUserIdByUsernameOrDefaultAsync(
            cmd.Username,
            cancellationToken
        );

        if (externalUserId == null)
        {
            throw NotFoundException.Entity(nameof(User), nameof(User.Username), cmd.Username);
        }

        var passwordResetTokenDto = await _accountPasswordManager.GetPasswordResetTokenAsync(
            externalUserId,
            cancellationToken
        );

        await _eventBus.PublishAsync(
            new PasswordResetRequestedNotification(
                externalUserId,
                passwordResetTokenDto.ResetToken
            ),
            cancellationToken
        );
    }
}
