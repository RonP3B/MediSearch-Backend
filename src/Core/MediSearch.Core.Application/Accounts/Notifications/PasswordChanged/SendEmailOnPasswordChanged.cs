using MediSearch.Core.Application.Accounts.Constants;
using MediSearch.Core.Application.Accounts.Models;
using MediSearch.Core.Application.Accounts.Ports;

namespace MediSearch.Core.Application.Accounts.Notifications.PasswordChanged;

public sealed class SendEmailOnPasswordChanged(
    IAccountQueryService accountQueryService,
    ITemplateRenderingService templateRenderingService,
    IEmailService emailService
) : NotificationHandler<PasswordChangedNotification>
{
    private readonly IAccountQueryService _accountQueryService = accountQueryService;
    private readonly ITemplateRenderingService _templateRenderingService = templateRenderingService;
    private readonly IEmailService _emailService = emailService;

    public override async Task Handle(
        PasswordChangedNotification notification,
        CancellationToken cancellationToken
    )
    {
        var user = await _accountQueryService.GetUserContactInfoByExternalUserIdAsync(
            notification.ExternalUserId,
            cancellationToken
        );

        var body = await _templateRenderingService.RenderHtmlAsync(
            new PasswordChangedModel(FullName: user.FullName),
            cancellationToken
        );

        await _emailService.SendAsync(
            new EmailMessage(
                user.Email,
                AccountEmailSubjectCodes.PasswordChangedEmailSubject,
                body,
                user.DisplayName
            ),
            notification.IdempotencyKey,
            cancellationToken
        );
    }
}
