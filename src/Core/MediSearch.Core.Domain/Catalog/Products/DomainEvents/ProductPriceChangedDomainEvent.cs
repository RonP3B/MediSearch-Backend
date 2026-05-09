namespace MediSearch.Core.Domain.Catalog.Products.DomainEvents;

public sealed class ProductPriceChangedDomainEvent(
    Guid productId,
    Guid companyId,
    string name,
    string previousPrice,
    string newPrice
) : DomainEvent
{
    public Guid ProductId { get; } = productId;
    public Guid CompanyId { get; } = companyId;
    public string Name { get; } = name;
    public string PreviousPrice { get; } = previousPrice;
    public string NewPrice { get; } = newPrice;
}
