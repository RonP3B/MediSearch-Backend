using MediSearch.Core.Application.Favorites.Constants;
using MediSearch.Core.Application.Favorites.Notifications.CompanyAddedToFavorites;
using MediSearch.Core.Application.Favorites.Ports;
using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Core.Application.Users.Ports;
using MediSearch.Core.Domain.Favorites.CompanyFavorites;

namespace MediSearch.Core.Application.Favorites.DomainEventHandlers.CompanyAddedToFavorites;

public sealed class NotifyCompanyOwnerAndManagersOnCompanyAddedToFavorites(
    IFavoriteQueryService favoriteQueryService,
    IUserQueryService userQueryService,
    IEmailService emailService,
    IEventBus eventBus,
    ICacheService cacheService
) : DomainEventHandler<CompanyAddedToFavoritesDomainEvent>
{
    private readonly IFavoriteQueryService _favoriteQueryService = favoriteQueryService;
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly IEmailService _emailService = emailService;
    private readonly IEventBus _eventBus = eventBus;
    private readonly ICacheService _cacheService = cacheService;

    public override async Task Handle(
        CompanyAddedToFavoritesDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        string cacheKey = FavoriteCacheKeys.CompanyAddedToFavoritesNotification(
            domainEvent.FavoriterAgentId,
            domainEvent.CompanyId
        );

        var alreadyNotified = await _cacheService.GetAsync<bool>(cacheKey, cancellationToken);

        if (alreadyNotified)
        {
            return;
        }

        var companyFavoritedBy = await _favoriteQueryService.GetCompanyFavoritedByAsync(
            domainEvent.CompanyId,
            domainEvent.FavoriterAgentId,
            domainEvent.FavoriterAgentTypeId,
            cancellationToken
        );

        UserContactInfoDto[] companyOwnerAndManagersContactInfo =
        [
            .. await _userQueryService.GetCompanyOwnerAndManagersContactInfoAsync(
                domainEvent.CompanyId,
                cancellationToken
            ),
        ];

        if (companyOwnerAndManagersContactInfo.Length == 0)
        {
            throw CorruptedInvariantException.MissingCompanyOwnerContact(domainEvent.CompanyId);
        }

        var notifications = companyOwnerAndManagersContactInfo
            .Chunk(_emailService.MaxBatchSize)
            .Select(
                (chunk, index) =>
                    new CompanyAddedToFavoritesNotification(
                        companyFavoritedBy.CompanyName,
                        companyFavoritedBy.FavoriterName,
                        chunk,
                        $"{domainEvent.Id}:company-added-to-favorites:{index}"
                    )
            )
            .ToArray();

        await _eventBus.PublishBatchAsync(notifications, cancellationToken);

        await _cacheService.SetAsync(cacheKey, true, TimeSpan.FromHours(1), cancellationToken);
    }
}
