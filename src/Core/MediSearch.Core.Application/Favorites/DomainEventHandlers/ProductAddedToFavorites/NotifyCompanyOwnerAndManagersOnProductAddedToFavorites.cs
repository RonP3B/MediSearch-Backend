using MediSearch.Core.Application.Favorites.Constants;
using MediSearch.Core.Application.Favorites.Notifications.ProductAddedToFavorites;
using MediSearch.Core.Application.Favorites.Ports;
using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Core.Application.Users.Ports;
using MediSearch.Core.Domain.Favorites.ProductFavorites;

namespace MediSearch.Core.Application.Favorites.DomainEventHandlers.ProductAddedToFavorites;

public sealed class NotifyCompanyOwnerAndManagersOnProductAddedToFavorites(
    IFavoriteQueryService favoriteQueryService,
    IUserQueryService userQueryService,
    IEmailService emailService,
    IEventBus eventBus,
    ICacheService cacheService
) : DomainEventHandler<ProductAddedToFavoritesDomainEvent>
{
    private readonly IFavoriteQueryService _favoriteQueryService = favoriteQueryService;
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly IEmailService _emailService = emailService;
    private readonly IEventBus _eventBus = eventBus;
    private readonly ICacheService _cacheService = cacheService;

    public override async Task Handle(
        ProductAddedToFavoritesDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        string cacheKey = FavoriteCacheKeys.ProductAddedToFavoritesNotification(
            domainEvent.FavoriterAgentId,
            domainEvent.ProductId
        );

        var alreadyNotified = await _cacheService.GetAsync<bool>(cacheKey, cancellationToken);

        if (alreadyNotified)
        {
            return;
        }

        var productFavoritedBy = await _favoriteQueryService.GetProductFavoritedByAsync(
            domainEvent.ProductId,
            domainEvent.FavoriterAgentId,
            domainEvent.FavoriterAgentTypeId,
            cancellationToken
        );

        UserContactInfoDto[] companyOwnerAndManagersContactInfo =
        [
            .. await _userQueryService.GetCompanyOwnerAndManagersContactInfoAsync(
                productFavoritedBy.CompanyId,
                cancellationToken
            ),
        ];

        if (companyOwnerAndManagersContactInfo.Length == 0)
        {
            throw CorruptedInvariantException.MissingCompanyOwnerContact(
                productFavoritedBy.CompanyId
            );
        }

        var notifications = companyOwnerAndManagersContactInfo
            .Chunk(_emailService.MaxBatchSize)
            .Select(
                (chunk, index) =>
                    new ProductAddedToFavoritesNotification(
                        productFavoritedBy.CompanyName,
                        productFavoritedBy.ProductName,
                        productFavoritedBy.FavoriterName,
                        chunk,
                        $"{domainEvent.Id}:product-added-to-favorites:{index}"
                    )
            )
            .ToArray();

        await _eventBus.PublishBatchAsync(notifications, cancellationToken);

        await _cacheService.SetAsync(cacheKey, true, TimeSpan.FromHours(1), cancellationToken);
    }
}
