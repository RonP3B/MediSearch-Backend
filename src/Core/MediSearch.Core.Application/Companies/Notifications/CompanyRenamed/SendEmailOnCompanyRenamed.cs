using MediSearch.Core.Application.Companies.Constants;
using MediSearch.Core.Application.Companies.Models;

namespace MediSearch.Core.Application.Companies.Notifications.CompanyRenamed;

public sealed class SendEmailOnCompanyRenamed(
    ITemplateRenderingService templateRenderingService,
    IEmailService emailService
) : NotificationHandler<CompanyRenamedNotification>
{
    private readonly ITemplateRenderingService _templateRenderingService = templateRenderingService;
    private readonly IEmailService _emailService = emailService;

    public override async Task Handle(
        CompanyRenamedNotification notification,
        CancellationToken cancellationToken
    )
    {
        string body = await _templateRenderingService.RenderHtmlAsync(
            new CompanyRenamedModel(
                PreviousCompanyName: notification.PreviousCompanyName,
                NewCompanyName: notification.NewCompanyName
            ),
            cancellationToken
        );

        var messages = notification.FavoritersContactInfo.Select(
            favoriterContactInfo => new EmailMessage(
                favoriterContactInfo.Email,
                CompanyEmailSubjectCodes.CompanyRenamedEmailSubject,
                body,
                favoriterContactInfo.DisplayName
            )
        );

        await _emailService.SendBulkAsync(messages, notification.IdempotencyKey, cancellationToken);
    }
}
