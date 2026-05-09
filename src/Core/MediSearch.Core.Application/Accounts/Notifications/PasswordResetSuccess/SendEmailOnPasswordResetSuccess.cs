using MediSearch.Core.Application.Accounts.Constants;
using MediSearch.Core.Application.Accounts.Models;
using MediSearch.Core.Application.Accounts.Ports;

namespace MediSearch.Core.Application.Accounts.Notifications.PasswordResetSuccess;

public sealed class SendEmailOnPasswordResetSuccess(
    IAccountQueryService accountQueryService,
    ITemplateRenderingService templateRenderingService,
    IEmailService emailService
) : NotificationHandler<PasswordResetSuccessNotification>
{
    private readonly IAccountQueryService _accountQueryService = accountQueryService;
    private readonly ITemplateRenderingService _templateRenderingService = templateRenderingService;
    private readonly IEmailService _emailService = emailService;

    public override async Task Handle(
        PasswordResetSuccessNotification notification,
        CancellationToken cancellationToken
    )
    {
        var user = await _accountQueryService.GetUserContactInfoByExternalUserIdAsync(
            notification.ExternalUserId,
            cancellationToken
        );

        var body = await _templateRenderingService.RenderHtmlAsync(
            new PasswordResetSuccessModel(FullName: user.FullName),
            cancellationToken
        );

        await _emailService.SendAsync(
            new EmailMessage(
                user.Email,
                AccountEmailSubjectCodes.PasswordResetConfirmedEmailSubject,
                body,
                user.DisplayName
            ),
            notification.IdempotencyKey,
            cancellationToken
        );
    }
}
