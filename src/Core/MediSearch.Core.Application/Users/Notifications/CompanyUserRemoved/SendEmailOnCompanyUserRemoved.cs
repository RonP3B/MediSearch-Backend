using MediSearch.Core.Application.Users.Constants;
using MediSearch.Core.Application.Users.Models;

namespace MediSearch.Core.Application.Users.Notifications.CompanyUserRemoved;

public sealed class SendEmailOnCompanyUserRemoved(
    ITemplateRenderingService templateRenderingService,
    IEmailService emailService
) : NotificationHandler<CompanyUserRemovedNotification>
{
    private readonly ITemplateRenderingService _templateRenderingService = templateRenderingService;
    private readonly IEmailService _emailService = emailService;

    public override async Task Handle(
        CompanyUserRemovedNotification notification,
        CancellationToken cancellationToken
    )
    {
        string body = await _templateRenderingService.RenderHtmlAsync(
            new CompanyUserRemovedModel(
                notification.FullName,
                notification.Username,
                notification.CompanyName
            ),
            cancellationToken
        );

        await _emailService.SendAsync(
            new EmailMessage(
                notification.Email,
                UserEmailSubjectCodes.CompanyUserRemovedEmailSubject,
                body,
                $"{notification.FullName} ({notification.Username})"
            ),
            notification.IdempotencyKey,
            cancellationToken
        );
    }
}
