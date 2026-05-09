using MediSearch.Core.Application.Favorites.Constants;
using MediSearch.Core.Application.Favorites.Models;

namespace MediSearch.Core.Application.Favorites.Notifications.ProductAddedToFavorites;

public sealed class SendEmailOnProductAddedToFavorites(
    ITemplateRenderingService templateRenderingService,
    IEmailService emailService
) : NotificationHandler<ProductAddedToFavoritesNotification>
{
    private readonly ITemplateRenderingService _templateRenderingService = templateRenderingService;
    private readonly IEmailService _emailService = emailService;

    public override async Task Handle(
        ProductAddedToFavoritesNotification notification,
        CancellationToken cancellationToken
    )
    {
        string body = await _templateRenderingService.RenderHtmlAsync(
            new ProductAddedToFavoritesModel(
                CompanyName: notification.CompanyName,
                ProductName: notification.ProductName,
                FavoriterName: notification.FavoriterName
            ),
            cancellationToken
        );

        var messages = notification.CompanyOwnerAndManagersContactInfo.Select(
            contactInfo => new EmailMessage(
                contactInfo.Email,
                FavoriteEmailSubjectCodes.ProductAddedToFavoritesEmailSubject,
                body,
                contactInfo.DisplayName
            )
        );

        await _emailService.SendBulkAsync(messages, notification.IdempotencyKey, cancellationToken);
    }
}
