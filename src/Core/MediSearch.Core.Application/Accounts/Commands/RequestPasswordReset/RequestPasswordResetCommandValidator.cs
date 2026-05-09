using FluentValidation;

namespace MediSearch.Core.Application.Accounts.Commands.RequestPasswordReset;

public sealed class RequestPasswordResetCommandValidator
    : AbstractValidator<RequestPasswordResetCommand>
{
    public RequestPasswordResetCommandValidator()
    {
        RuleFor(v => v.Username).NotEmpty();
    }
}
