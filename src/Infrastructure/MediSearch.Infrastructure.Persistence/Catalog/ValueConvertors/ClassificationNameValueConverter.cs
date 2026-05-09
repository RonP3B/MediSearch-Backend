using MediSearch.Core.Domain.Catalog.ProductClassifications;

namespace MediSearch.Infrastructure.Persistence.Catalog.ValueConvertors;

internal sealed class ClassificationNameValueConverter : ValueConverter<Name, string>
{
    public ClassificationNameValueConverter()
        : base(name => name.Value, value => Name.From(value)) { }
}
