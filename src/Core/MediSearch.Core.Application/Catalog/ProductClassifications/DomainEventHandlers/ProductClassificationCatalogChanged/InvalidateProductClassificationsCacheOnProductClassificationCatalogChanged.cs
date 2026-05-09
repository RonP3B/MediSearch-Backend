using MediSearch.Core.Application.Catalog.ProductClassifications.Constants;
using MediSearch.Core.Domain.Catalog.ProductClassifications.DomainEvents;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.DomainEventHandlers.ProductClassificationCatalogChanged;

public sealed class InvalidateProductClassificationsCacheOnProductClassificationCatalogChanged(
    ICacheService cacheService
) : DomainEventHandler<ProductClassificationCatalogChangedDomainEvent>
{
    private readonly ICacheService _cacheService = cacheService;

    public override Task Handle(
        ProductClassificationCatalogChangedDomainEvent domainEvent,
        CancellationToken cancellationToken
    ) =>
        _cacheService.RemoveAsync(
            ProductClassificationCacheKeys.AllProductClassifications,
            cancellationToken
        );
}
