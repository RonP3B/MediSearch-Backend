using MediSearch.Core.Application.Catalog.Products.Notifications.ProductPriceChangedForFavoriters;
using MediSearch.Core.Application.Favorites.Ports;
using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Core.Domain.Catalog.Products.DomainEvents;

namespace MediSearch.Core.Application.Catalog.Products.DomainEventHandlers.ProductPriceChanged;

public sealed class NotifyFavoritersOnProductPriceChanged(
    IFavoriteQueryService favoriteQueryService,
    IEmailService emailService,
    IEventBus eventBus
) : DomainEventHandler<ProductPriceChangedDomainEvent>
{
    private readonly IFavoriteQueryService _favoriteQueryService = favoriteQueryService;
    private readonly IEmailService _emailService = emailService;
    private readonly IEventBus _eventBus = eventBus;

    public override async Task Handle(
        ProductPriceChangedDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        UserContactInfoDto[] favoritersContactInfo =
        [
            .. await _favoriteQueryService.GetProductFavoritersContactInfoAsync(
                domainEvent.ProductId,
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
                    new ProductPriceChangedForFavoritersNotification(
                        domainEvent.Name,
                        domainEvent.PreviousPrice,
                        domainEvent.NewPrice,
                        chunk,
                        $"{domainEvent.Id}:product-price-changed-favoriters:{index}"
                    )
            )
            .ToArray();

        await _eventBus.PublishBatchAsync(notifications, cancellationToken);
    }
}
