using MediSearch.Core.Domain.Users.ValueObjects;

namespace MediSearch.Infrastructure.Persistence.Users.ValueConverters;

internal sealed class ExternalIdValueConverter : ValueConverter<ExternalId, string>
{
    public ExternalIdValueConverter()
        : base(externalId => externalId.Value, value => ExternalId.From(value)) { }
}
