using MediSearch.Core.Domain.Companies.ValueObjects;

namespace MediSearch.Infrastructure.Persistence.Companies.ValueConverters;

internal sealed class NullableUrlValueConverter
    : NullableSingleFieldValueObjectConverter<Url, string>
{
    public NullableUrlValueConverter()
        : base(url => url.Value, Url.From) { }
}
