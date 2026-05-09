namespace MediSearch.Core.Domain.Catalog.Products.DomainEvents;

public sealed class ProductOutOfStockDomainEvent(Guid productId, Guid companyId, string name)
    : DomainEvent
{
    public Guid ProductId { get; } = productId;
    public Guid CompanyId { get; } = companyId;
    public string Name { get; } = name;
}
