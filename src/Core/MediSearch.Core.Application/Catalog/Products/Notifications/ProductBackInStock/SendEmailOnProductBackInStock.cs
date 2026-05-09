using MediSearch.Core.Application.Catalog.Products.Constants;
using MediSearch.Core.Application.Catalog.Products.Models;

namespace MediSearch.Core.Application.Catalog.Products.Notifications.ProductBackInStock;

public sealed class SendEmailOnProductBackInStock(
    ITemplateRenderingService templateRenderingService,
    IEmailService emailService
) : NotificationHandler<ProductBackInStockNotification>
{
    private readonly ITemplateRenderingService _templateRenderingService = templateRenderingService;
    private readonly IEmailService _emailService = emailService;

    public override async Task Handle(
        ProductBackInStockNotification notification,
        CancellationToken cancellationToken
    )
    {
        string body = await _templateRenderingService.RenderHtmlAsync(
            new ProductBackInStockModel(notification.ProductName),
            cancellationToken
        );

        var messages = notification.FavoritersContactInfo.Select(
            favoriterContactInfo => new EmailMessage(
                favoriterContactInfo.Email,
                ProductEmailSubjectCodes.ProductBackInStockEmailSubject,
                body,
                favoriterContactInfo.DisplayName
            )
        );

        await _emailService.SendBulkAsync(messages, notification.IdempotencyKey, cancellationToken);
    }
}
