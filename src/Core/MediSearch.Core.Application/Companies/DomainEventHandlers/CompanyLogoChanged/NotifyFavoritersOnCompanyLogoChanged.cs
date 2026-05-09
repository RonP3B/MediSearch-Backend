using MediSearch.Core.Application.Companies.Notifications.CompanyLogoChanged;
using MediSearch.Core.Application.Favorites.Ports;
using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Core.Domain.Companies.DomainEvents;

namespace MediSearch.Core.Application.Companies.DomainEventHandlers.CompanyLogoChanged;

public sealed class NotifyFavoritersOnCompanyLogoChanged(
    IFavoriteQueryService favoriteQueryService,
    IEmailService emailService,
    IEventBus eventBus
) : DomainEventHandler<CompanyLogoChangedDomainEvent>
{
    private readonly IFavoriteQueryService _favoriteQueryService = favoriteQueryService;
    private readonly IEmailService _emailService = emailService;
    private readonly IEventBus _eventBus = eventBus;

    public override async Task Handle(
        CompanyLogoChangedDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        UserContactInfoDto[] favoritersContactInfo =
        [
            .. await _favoriteQueryService.GetCompanyFavoritersContactInfoAsync(
                domainEvent.CompanyId,
                cancellationToken
            ),
        ];

        if (favoritersContactInfo.Length == 0)
        {
            return;
        }

        var notifications = favoritersContactInfo
            .Chunk(_emailService.MaxBatchSize)
            .Select(
                (chunk, index) =>
                    new CompanyLogoChangedNotification(
                        domainEvent.CompanyName,
                        domainEvent.NewImageKey,
                        chunk,
                        $"{domainEvent.Id}:company-logo-changed:{index}"
                    )
            )
            .ToArray();

        await _eventBus.PublishBatchAsync(notifications, cancellationToken);
    }
}
