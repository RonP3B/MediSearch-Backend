using EmailVO = MediSearch.Core.Domain.SharedKernel.ValueObjects.Email;

namespace MediSearch.Infrastructure.Persistence.Shared.ValueConverters;

internal sealed class EmailValueConverter : ValueConverter<EmailVO, string>
{
    public EmailValueConverter()
        : base(email => email.Address, value => EmailVO.From(value)) { }
}
