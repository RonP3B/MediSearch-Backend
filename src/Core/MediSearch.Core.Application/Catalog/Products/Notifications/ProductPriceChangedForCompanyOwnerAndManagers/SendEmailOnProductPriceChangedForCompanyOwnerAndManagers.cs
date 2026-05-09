using MediSearch.Core.Application.Catalog.Products.Constants;
using MediSearch.Core.Application.Catalog.Products.Models;

namespace MediSearch.Core.Application.Catalog.Products.Notifications.ProductPriceChangedForCompanyOwnerAndManagers;

public sealed class SendEmailOnProductPriceChangedForCompanyOwnerAndManagers(
    ITemplateRenderingService templateRenderingService,
    IEmailService emailService
) : NotificationHandler<ProductPriceChangedForCompanyOwnerAndManagersNotification>
{
    private readonly ITemplateRenderingService _templateRenderingService = templateRenderingService;
    private readonly IEmailService _emailService = emailService;

    public override async Task Handle(
        ProductPriceChangedForCompanyOwnerAndManagersNotification notification,
        CancellationToken cancellationToken
    )
    {
        string body = await _templateRenderingService.RenderHtmlAsync(
            new ProductPriceChangedForCompanyOwnerAndManagersModel(
                notification.ProductName,
                notification.PreviousPrice,
                notification.NewPrice
            ),
            cancellationToken
        );

        var messages = notification.CompanyOwnerAndManagersContactInfo.Select(
            contactInfo => new EmailMessage(
                contactInfo.Email,
                ProductEmailSubjectCodes.ProductPriceChangedForCompanyOwnerAndManagersEmailSubject,
                body,
                contactInfo.DisplayName
            )
        );

        await _emailService.SendBulkAsync(messages, notification.IdempotencyKey, cancellationToken);
    }
}
