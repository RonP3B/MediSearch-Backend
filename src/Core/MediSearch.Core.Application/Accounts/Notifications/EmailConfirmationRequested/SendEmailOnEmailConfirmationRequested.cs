using MediSearch.Core.Application.Accounts.Constants;
using MediSearch.Core.Application.Accounts.Models;
using MediSearch.Core.Application.Accounts.Ports;

namespace MediSearch.Core.Application.Accounts.Notifications.EmailConfirmationRequested;

public sealed class SendEmailOnEmailConfirmationRequested(
    IAccountQueryService accountQueryService,
    ITemplateRenderingService templateRenderingService,
    IEmailService emailService
) : NotificationHandler<EmailConfirmationRequestedNotification>
{
    private readonly IAccountQueryService _accountQueryService = accountQueryService;
    private readonly ITemplateRenderingService _templateRenderingService = templateRenderingService;
    private readonly IEmailService _emailService = emailService;

    public override async Task Handle(
        EmailConfirmationRequestedNotification notification,
        CancellationToken cancellationToken
    )
    {
        var contactInfo = await _accountQueryService.GetUserContactInfoByExternalUserIdAsync(
            notification.ExternalUserId,
            cancellationToken
        );

        string body = await _templateRenderingService.RenderHtmlAsync(
            new AccountEmailConfirmationRequestedModel(
                ExternalUserId: notification.ExternalUserId,
                ConfirmationToken: notification.ConfirmationToken,
                FullName: contactInfo.FullName
            ),
            cancellationToken
        );

        await _emailService.SendAsync(
            new EmailMessage(
                contactInfo.Email,
                AccountEmailSubjectCodes.EmailConfirmationRequestedSubject,
                body,
                contactInfo.DisplayName
            ),
            notification.IdempotencyKey,
            cancellationToken
        );
    }
}
