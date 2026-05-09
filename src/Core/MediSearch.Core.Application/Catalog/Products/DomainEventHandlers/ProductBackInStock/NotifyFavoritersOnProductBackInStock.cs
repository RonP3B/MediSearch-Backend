using MediSearch.Core.Application.Catalog.Products.Notifications.ProductBackInStock;
using MediSearch.Core.Application.Favorites.Ports;
using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Core.Domain.Catalog.Products.DomainEvents;

namespace MediSearch.Core.Application.Catalog.Products.DomainEventHandlers.ProductBackInStock;

public sealed class NotifyFavoritersOnProductBackInStock(
    IFavoriteQueryService favoriteQueryService,
    IEmailService emailService,
    IEventBus eventBus
) : DomainEventHandler<ProductBackInStockDomainEvent>
{
    private readonly IFavoriteQueryService _favoriteQueryService = favoriteQueryService;
    private readonly IEmailService _emailService = emailService;
    private readonly IEventBus _eventBus = eventBus;

    public override async Task Handle(
        ProductBackInStockDomainEvent domainEvent,
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
                    new ProductBackInStockNotification(
                        domainEvent.Name,
                        chunk,
                        $"{domainEvent.Id}:product-back-in-stock:{index}"
                    )
            )
            .ToArray();

        await _eventBus.PublishBatchAsync(notifications, cancellationToken);
    }
}
