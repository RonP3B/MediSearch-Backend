using MediSearch.Core.Application.Shared.Compensations.Events.AssetsDeletion;
using MediSearch.Core.Domain.Catalog.Products.DomainEvents;

namespace MediSearch.Core.Application.Catalog.Products.DomainEventHandlers.ProductDeleted;

public sealed class AssetsCompensationOnProductDeleted(IEventBus eventBus)
    : DomainEventHandler<ProductDeletedDomainEvent>
{
    private readonly IEventBus _eventBus = eventBus;

    public override async Task Handle(
        ProductDeletedDomainEvent domainEvent,
        CancellationToken cancellationToken
    )
    {
        await _eventBus.PublishAsync(
            new AssetsDeletionCompensationEvent([.. domainEvent.ImageKeys]),
            cancellationToken
        );
    }
}
