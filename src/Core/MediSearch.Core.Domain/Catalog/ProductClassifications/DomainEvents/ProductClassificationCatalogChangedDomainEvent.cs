namespace MediSearch.Core.Domain.Catalog.ProductClassifications.DomainEvents;

public sealed class ProductClassificationCatalogChangedDomainEvent(Guid classificationId)
    : DomainEvent
{
    public Guid ClassificationId { get; } = classificationId;
}
