namespace MediSearch.Core.Domain.Catalog.Products.DomainEvents;

public sealed class ProductCreatedDomainEvent(
    Guid productId,
    Guid companyId,
    string name,
    string price
) : DomainEvent
{
    public Guid ProductId { get; } = productId;
    public Guid CompanyId { get; } = companyId;
    public string Name { get; } = name;
    public string Price { get; } = price;
}
