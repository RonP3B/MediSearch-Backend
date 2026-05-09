using MediSearch.Core.Domain.Catalog.Products.ValueObjects;

namespace MediSearch.Infrastructure.Persistence.Catalog.ValueConvertors;

internal sealed class QuantityValueConverter : ValueConverter<Quantity, int>
{
    public QuantityValueConverter()
        : base(quantity => quantity.Value, value => Quantity.From(value)) { }
}
