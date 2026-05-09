namespace MediSearch.Core.Domain.Catalog.Products.DomainEvents;

public sealed class ProductDeletedDomainEvent(
    Guid productId,
    Guid companyId,
    string name,
    IReadOnlyList<string> imageKeys
) : DomainEvent
{
    public Guid ProductId { get; } = productId;
    public Guid CompanyId { get; } = companyId;
    public string Name { get; } = name;
    public IReadOnlyList<string> ImageKeys { get; } = imageKeys;
}
