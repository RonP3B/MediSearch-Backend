using FluentValidation;

namespace MediSearch.Core.Application.Accounts.Commands.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(v => v.Username).NotEmpty();

        RuleFor(v => v.Password).NotEmpty();
    }
}
