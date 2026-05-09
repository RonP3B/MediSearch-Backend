using FluentValidation;
using MediSearch.Core.Application.Users.Constants;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;
using MediSearch.Core.Domain.Users;
using MediSearch.Core.Domain.Users.ValueObjects;

namespace MediSearch.Core.Application.Users.Commands.RegisterCompanyUser;

public sealed class RegisterCompanyUserCommandValidator
    : AbstractValidator<RegisterCompanyUserCommand>
{
    private readonly IUserRepository _userRepository;

    private static readonly int[] AllowedRoleIds = [Role.CompanyManager.Id, Role.CompanyMember.Id];

    public RegisterCompanyUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(v => v).ValidValueObject(cmd => FullName.TryFrom(cmd.FirstName, cmd.LastName));

        RuleFor(v => v.PhoneNumber).ValidValueObject(PhoneNumber.TryFrom);

        RuleFor(v => v)
            .ValidValueObject(cmd => Location.TryFrom(cmd.Province, cmd.Municipality, cmd.Address));

        RuleFor(v => v.Email)
            .ValidValueObject(Email.TryFrom)
            .DependentRules(() => RuleFor(v => v.Email).UniqueEmail(_userRepository));

        RuleFor(v => v.RoleInCompany)
            .Must(roleId => AllowedRoleIds.Contains(roleId))
            .WithCustomErrorCode(UserErrorCodes.InvalidRoleForCompanyUser);
    }
}
