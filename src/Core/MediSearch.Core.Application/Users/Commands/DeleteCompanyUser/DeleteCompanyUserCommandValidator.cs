using FluentValidation;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;
using MediSearch.Core.Domain.Users;

namespace MediSearch.Core.Application.Users.Commands.DeleteCompanyUser;

public sealed class DeleteCompanyUserCommandValidator : AbstractValidator<DeleteCompanyUserCommand>
{
    public DeleteCompanyUserCommandValidator()
    {
        RuleFor(v => v.UserId).ValidValueObject(EntityId<User>.TryFrom);
    }
}
