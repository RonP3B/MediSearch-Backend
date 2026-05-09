using FluentValidation;

namespace MediSearch.Core.Application.Accounts.Commands.ConfirmAccountEmail;

public sealed class ConfirmAccountEmailCommandValidator
    : AbstractValidator<ConfirmAccountEmailCommand>
{
    public ConfirmAccountEmailCommandValidator()
    {
        RuleFor(v => v.ExternalUserId).NotEmpty();

        RuleFor(v => v.ConfirmationToken).NotEmpty();
    }
}
