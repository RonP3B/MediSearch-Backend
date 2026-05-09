using MediSearch.Core.Domain.Catalog.Products.ValueObjects;

namespace MediSearch.Infrastructure.Persistence.Catalog.ValueConvertors;

internal sealed class ProductNameValueConverter : ValueConverter<ProductName, string>
{
    public ProductNameValueConverter()
        : base(productName => productName.Value, value => ProductName.From(value)) { }
}
