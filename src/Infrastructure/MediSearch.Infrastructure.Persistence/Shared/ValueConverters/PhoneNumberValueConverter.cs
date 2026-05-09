using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Infrastructure.Persistence.Shared.ValueConverters;

internal sealed class PhoneNumberValueConverter : ValueConverter<PhoneNumber, string>
{
    public PhoneNumberValueConverter()
        : base(phoneNumber => phoneNumber.Value, value => PhoneNumber.From(value)) { }
}
