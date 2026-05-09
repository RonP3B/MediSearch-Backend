using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Infrastructure.Persistence.Chat;

internal sealed class NullableCleanTextValueConverter
    : NullableSingleFieldValueObjectConverter<CleanText, string>
{
    public NullableCleanTextValueConverter()
        : base(cleanText => cleanText.Value, CleanText.From) { }
}
