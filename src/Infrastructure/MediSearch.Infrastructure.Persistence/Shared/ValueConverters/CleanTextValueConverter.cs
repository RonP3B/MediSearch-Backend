using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Infrastructure.Persistence.Shared.ValueConverters;

internal sealed class CleanTextValueConverter : ValueConverter<CleanText, string>
{
    public CleanTextValueConverter()
        : base(cleanText => cleanText.Value, value => CleanText.From(value)) { }
}
