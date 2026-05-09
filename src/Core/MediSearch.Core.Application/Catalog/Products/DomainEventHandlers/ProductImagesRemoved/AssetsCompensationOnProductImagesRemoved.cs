using MediSearch.Core.Application.Shared.Compensations.Events.AssetsDeletion;
using MediSearch.Core.Domain.Catalog.Products.DomainEvents;

namespace MediSearch.Core.Application.Catalog.Products.DomainEventHandlers.ProductImagesRemoved;

public sealed class AssetsCompensationOnProductImagesRemoved(IEventBus eventBus)
    : DomainEventHandler<ProductImagesRemovedDomainEvent>
{
    private readonly IEventBus _eventBus = eventBus;

    public override async Task Handle(
        ProductImagesRemovedDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        await _eventBus.PublishAsync(
            new AssetsDeletionCompensationEvent([.. domainEvent.RemovedImageKeys]),
            cancellationToken
        );
    }
}
