using MediSearch.Core.Application.Catalog.Products.Constants;
using MediSearch.Core.Application.Catalog.Products.Models;

namespace MediSearch.Core.Application.Catalog.Products.Notifications.ProductPriceChangedForFavoriters;

public sealed class SendEmailOnProductPriceChangedForFavoriters(
    ITemplateRenderingService templateRenderingService,
    IEmailService emailService
) : NotificationHandler<ProductPriceChangedForFavoritersNotification>
{
    private readonly ITemplateRenderingService _templateRenderingService = templateRenderingService;
    private readonly IEmailService _emailService = emailService;

    public override async Task Handle(
        ProductPriceChangedForFavoritersNotification notification,
        CancellationToken cancellationToken
    )
    {
        string body = await _templateRenderingService.RenderHtmlAsync(
            new ProductPriceChangedForFavoritersModel(
                notification.ProductName,
                notification.PreviousPrice,
                notification.NewPrice
            ),
            cancellationToken
        );

        var messages = notification.FavoritersContactInfo.Select(
            favoriterContactInfo => new EmailMessage(
                favoriterContactInfo.Email,
                ProductEmailSubjectCodes.ProductPriceChangedForFavoritersEmailSubject,
                body,
                favoriterContactInfo.DisplayName
            )
        );

        await _emailService.SendBulkAsync(messages, notification.IdempotencyKey, cancellationToken);
    }
}
