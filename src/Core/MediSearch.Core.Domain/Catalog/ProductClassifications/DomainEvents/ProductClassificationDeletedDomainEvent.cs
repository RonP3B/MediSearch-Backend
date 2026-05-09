namespace MediSearch.Core.Domain.Catalog.ProductClassifications.DomainEvents;

public sealed class ProductClassificationDeletedDomainEvent(Guid classificationId) : DomainEvent
{
    public Guid ClassificationId { get; } = classificationId;
}
