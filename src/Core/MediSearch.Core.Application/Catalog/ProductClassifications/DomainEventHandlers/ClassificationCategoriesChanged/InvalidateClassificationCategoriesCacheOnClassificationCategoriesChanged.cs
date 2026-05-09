using MediSearch.Core.Application.Catalog.ProductClassifications.Constants;
using MediSearch.Core.Domain.Catalog.ProductClassifications.DomainEvents;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.DomainEventHandlers.ClassificationCategoriesChanged;

public sealed class InvalidateClassificationCategoriesCacheOnClassificationCategoriesChanged(
    ICacheService cacheService
) : DomainEventHandler<ClassificationCategoriesChangedDomainEvent>
{
    private readonly ICacheService _cacheService = cacheService;

    public override Task Handle(
        ClassificationCategoriesChangedDomainEvent domainEvent,
        CancellationToken cancellationToken
    ) =>
        _cacheService.RemoveAsync(
            ProductClassificationCacheKeys.ClassificationCategories(domainEvent.ClassificationId),
            cancellationToken
        );
}
