using MediSearch.Core.Domain.Companies.ValueObjects;

namespace MediSearch.Infrastructure.Persistence.Companies.ValueConverters;

internal sealed class CompanyNameValueConverter : ValueConverter<CompanyName, string>
{
    public CompanyNameValueConverter()
        : base(companyName => companyName.Value, value => CompanyName.From(value)) { }
}
