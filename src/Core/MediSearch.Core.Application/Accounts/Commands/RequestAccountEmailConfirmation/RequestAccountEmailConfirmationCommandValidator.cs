using FluentValidation;

namespace MediSearch.Core.Application.Accounts.Commands.RequestAccountEmailConfirmation;

public sealed class RequestAccountEmailConfirmationCommandValidator
    : AbstractValidator<RequestAccountEmailConfirmationCommand>
{
    public RequestAccountEmailConfirmationCommandValidator()
    {
        RuleFor(v => v.Username).NotEmpty();
    }
}
