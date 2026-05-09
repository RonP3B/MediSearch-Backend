namespace MediSearch.Core.Domain.Catalog.Products.DomainEvents;

public sealed class ProductImagesRemovedDomainEvent(
    Guid productId,
    IReadOnlyList<string> removedImageKeys
) : DomainEvent
{
    public Guid ProductId { get; } = productId;
    public IReadOnlyList<string> RemovedImageKeys { get; } = removedImageKeys;
}
