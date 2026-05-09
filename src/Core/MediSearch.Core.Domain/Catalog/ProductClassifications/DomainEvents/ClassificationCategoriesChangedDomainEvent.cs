namespace MediSearch.Core.Domain.Catalog.ProductClassifications.DomainEvents;

public sealed class ClassificationCategoriesChangedDomainEvent(Guid classificationId) : DomainEvent
{
    public Guid ClassificationId { get; } = classificationId;
}
