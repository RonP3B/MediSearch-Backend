using MediSearch.Core.Domain.Companies.ValueObjects;

namespace MediSearch.Infrastructure.Persistence.Companies.ValueConverters;

internal sealed class CompanyCeoNameValueConverter : ValueConverter<CompanyCeoName, string>
{
    public CompanyCeoNameValueConverter()
        : base(companyCeoName => companyCeoName.Value, value => CompanyCeoName.From(value)) { }
}
