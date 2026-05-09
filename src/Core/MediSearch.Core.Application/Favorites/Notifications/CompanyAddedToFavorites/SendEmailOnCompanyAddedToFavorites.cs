using MediSearch.Core.Application.Favorites.Constants;
using MediSearch.Core.Application.Favorites.Models;

namespace MediSearch.Core.Application.Favorites.Notifications.CompanyAddedToFavorites;

public sealed class SendEmailOnCompanyAddedToFavorites(
    ITemplateRenderingService templateRenderingService,
    IEmailService emailService
) : NotificationHandler<CompanyAddedToFavoritesNotification>
{
    private readonly ITemplateRenderingService _templateRenderingService = templateRenderingService;
    private readonly IEmailService _emailService = emailService;

    public override async Task Handle(
        CompanyAddedToFavoritesNotification notification,
        CancellationToken cancellationToken
    )
    {
        string body = await _templateRenderingService.RenderHtmlAsync(
            new CompanyAddedToFavoritesModel(
                CompanyName: notification.CompanyName,
                FavoriterName: notification.FavoriterName
            ),
            cancellationToken
        );

        var messages = notification.CompanyOwnerAndManagersContactInfo.Select(
            contactInfo => new EmailMessage(
                contactInfo.Email,
                FavoriteEmailSubjectCodes.CompanyAddedToFavoritesEmailSubject,
                body,
                contactInfo.DisplayName
            )
        );

        await _emailService.SendBulkAsync(messages, notification.IdempotencyKey, cancellationToken);
    }
}
