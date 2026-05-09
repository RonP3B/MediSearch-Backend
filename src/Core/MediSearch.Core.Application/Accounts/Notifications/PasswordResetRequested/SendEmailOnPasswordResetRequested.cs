using MediSearch.Core.Application.Accounts.Constants;
using MediSearch.Core.Application.Accounts.Models;
using MediSearch.Core.Application.Accounts.Ports;

namespace MediSearch.Core.Application.Accounts.Notifications.PasswordResetRequested;

public sealed class SendEmailOnPasswordResetRequested(
    IAccountQueryService accountQueryService,
    ITemplateRenderingService templateRenderingService,
    IEmailService emailService
) : NotificationHandler<PasswordResetRequestedNotification>
{
    private readonly IAccountQueryService _accountQueryService = accountQueryService;
    private readonly ITemplateRenderingService _templateRenderingService = templateRenderingService;
    private readonly IEmailService _emailService = emailService;

    public override async Task Handle(
        PasswordResetRequestedNotification notification,
        CancellationToken cancellationToken
    )
    {
        var contactInfo = await _accountQueryService.GetUserContactInfoByExternalUserIdAsync(
            notification.ExternalUserId,
            cancellationToken
        );

        var body = await _templateRenderingService.RenderHtmlAsync(
            new PasswordResetRequestedModel(
                notification.ExternalUserId,
                notification.ResetToken,
                contactInfo.FullName
            ),
            cancellationToken
        );

        await _emailService.SendAsync(
            new EmailMessage(
                contactInfo.Email,
                AccountEmailSubjectCodes.PasswordResetRequestedEmailSubject,
                body,
                contactInfo.DisplayName
            ),
            notification.IdempotencyKey,
            cancellationToken
        );
    }
}
