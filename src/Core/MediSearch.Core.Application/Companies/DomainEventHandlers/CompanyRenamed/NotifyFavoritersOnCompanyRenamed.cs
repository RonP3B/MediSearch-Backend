using MediSearch.Core.Application.Companies.Notifications.CompanyRenamed;
using MediSearch.Core.Application.Favorites.Ports;
using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Core.Domain.Companies.DomainEvents;

namespace MediSearch.Core.Application.Companies.DomainEventHandlers.CompanyRenamed;

public sealed class NotifyFavoritersOnCompanyRenamed(
    IFavoriteQueryService favoriteQueryService,
    IEmailService emailService,
    IEventBus eventBus
) : DomainEventHandler<CompanyRenamedDomainEvent>
{
    private readonly IFavoriteQueryService _favoriteQueryService = favoriteQueryService;
    private readonly IEmailService _emailService = emailService;
    private readonly IEventBus _eventBus = eventBus;

    public override async Task Handle(
        CompanyRenamedDomainEvent domainEvent,
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
                    new CompanyRenamedNotification(
                        domainEvent.PreviousCompanyName,
                        domainEvent.NewCompanyName,
                        chunk,
                        $"{domainEvent.Id}:company-renamed:{index}"
                    )
            )
            .ToArray();

        await _eventBus.PublishBatchAsync(notifications, cancellationToken);
    }
}
