using MediSearch.Core.Application.Catalog.Products.Constants;
using MediSearch.Core.Application.Catalog.Products.Models;

namespace MediSearch.Core.Application.Catalog.Products.Notifications.ProductDeleted;

public sealed class SendEmailOnProductDeleted(
    ITemplateRenderingService templateRenderingService,
    IEmailService emailService
) : NotificationHandler<ProductDeletedNotification>
{
    private readonly ITemplateRenderingService _templateRenderingService = templateRenderingService;
    private readonly IEmailService _emailService = emailService;

    public override async Task Handle(
        ProductDeletedNotification notification,
        CancellationToken cancellationToken
    )
    {
        string body = await _templateRenderingService.RenderHtmlAsync(
            new ProductDeletedModel(notification.ProductName),
            cancellationToken
        );

        var messages = notification.CompanyOwnerAndManagersContactInfo.Select(
            contactInfo => new EmailMessage(
                contactInfo.Email,
                ProductEmailSubjectCodes.ProductDeletedEmailSubject,
                body,
                contactInfo.DisplayName
            )
        );

        await _emailService.SendBulkAsync(messages, notification.IdempotencyKey, cancellationToken);
    }
}
