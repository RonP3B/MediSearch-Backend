using FluentValidation;

namespace MediSearch.Core.Application.Accounts.Commands.RefreshAccessToken;

public sealed class RefreshAccessTokenCommandValidator
    : AbstractValidator<RefreshAccessTokenCommand>
{
    public RefreshAccessTokenCommandValidator()
    {
        RuleFor(v => v.RefreshToken).NotEmpty();
    }
}
