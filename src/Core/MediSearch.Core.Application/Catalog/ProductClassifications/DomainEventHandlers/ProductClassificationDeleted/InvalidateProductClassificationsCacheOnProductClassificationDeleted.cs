using MediSearch.Core.Application.Catalog.ProductClassifications.Constants;
using MediSearch.Core.Domain.Catalog.ProductClassifications.DomainEvents;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.DomainEventHandlers.ProductClassificationDeleted;

public sealed class InvalidateProductClassificationsCacheOnProductClassificationDeleted(
    ICacheService cacheService
) : DomainEventHandler<ProductClassificationDeletedDomainEvent>
{
    private readonly ICacheService _cacheService = cacheService;

    public override Task Handle(
        ProductClassificationDeletedDomainEvent domainEvent,
        CancellationToken cancellationToken
    ) =>
        _cacheService.RemoveAsync(
            ProductClassificationCacheKeys.AllProductClassifications,
            cancellationToken
        );
}
