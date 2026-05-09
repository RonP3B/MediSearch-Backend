using MediSearch.Core.Application.Catalog.Products.Constants;
using MediSearch.Core.Application.Catalog.Products.Models;

namespace MediSearch.Core.Application.Catalog.Products.Notifications.ProductOutOfStock;

public sealed class SendEmailOnProductOutOfStock(
    ITemplateRenderingService templateRenderingService,
    IEmailService emailService
) : NotificationHandler<ProductOutOfStockNotification>
{
    private readonly ITemplateRenderingService _templateRenderingService = templateRenderingService;
    private readonly IEmailService _emailService = emailService;

    public override async Task Handle(
        ProductOutOfStockNotification notification,
        CancellationToken cancellationToken
    )
    {
        string body = await _templateRenderingService.RenderHtmlAsync(
            new ProductOutOfStockModel(notification.ProductName),
            cancellationToken
        );

        var messages = notification.CompanyOwnerAndManagersContactInfo.Select(
            contactInfo => new EmailMessage(
                contactInfo.Email,
                ProductEmailSubjectCodes.ProductOutOfStockEmailSubject,
                body,
                contactInfo.DisplayName
            )
        );

        await _emailService.SendBulkAsync(messages, notification.IdempotencyKey, cancellationToken);
    }
}
