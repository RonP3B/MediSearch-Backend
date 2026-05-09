using FluentValidation;
using MediSearch.Core.Application.Accounts.Constants;

namespace MediSearch.Core.Application.Accounts.Commands.ChangePassword;

public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(v => v.CurrentPassword).NotEmpty();

        RuleFor(v => v.NewPassword)
            .NotEmpty()
            .DependentRules(() =>
                RuleFor(v => v.NewPassword)
                    .Password()
                    .Must((cmd, newPassword) => newPassword != cmd.CurrentPassword)
                    .WithCustomErrorCode(AccountErrorCodes.NewPasswordMustNotMatchOldOne)
            );
    }
}
