using FluentValidation;

namespace MediSearch.Core.Application.Accounts.Commands.ResetPassword;

public sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(v => v.NewPassword)
            .NotEmpty()
            .DependentRules(() => RuleFor(v => v.NewPassword).Password());

        RuleFor(v => v.ResetToken).NotEmpty();

        RuleFor(v => v.ExternalUserId).NotEmpty();
    }
}
