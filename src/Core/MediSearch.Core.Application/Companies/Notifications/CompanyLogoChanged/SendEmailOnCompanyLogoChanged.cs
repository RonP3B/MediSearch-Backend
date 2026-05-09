using MediSearch.Core.Application.Companies.Constants;
using MediSearch.Core.Application.Companies.Models;

namespace MediSearch.Core.Application.Companies.Notifications.CompanyLogoChanged;

public sealed class SendEmailOnCompanyLogoChanged(
    ITemplateRenderingService templateRenderingService,
    IEmailService emailService
) : NotificationHandler<CompanyLogoChangedNotification>
{
    private readonly ITemplateRenderingService _templateRenderingService = templateRenderingService;
    private readonly IEmailService _emailService = emailService;

    public override async Task Handle(
        CompanyLogoChangedNotification notification,
        CancellationToken cancellationToken
    )
    {
        string body = await _templateRenderingService.RenderHtmlAsync(
            new CompanyLogoChangedModel(
                CompanyName: notification.CompanyName,
                CompanyNewImageKey: notification.NewImageKey
            ),
            cancellationToken
        );

        var messages = notification.FavoritersContactInfo.Select(
            favoriterContactInfo => new EmailMessage(
                favoriterContactInfo.Email,
                CompanyEmailSubjectCodes.CompanyLogoChangedEmailSubject,
                body,
                favoriterContactInfo.DisplayName
            )
        );

        await _emailService.SendBulkAsync(messages, notification.IdempotencyKey, cancellationToken);
    }
}
