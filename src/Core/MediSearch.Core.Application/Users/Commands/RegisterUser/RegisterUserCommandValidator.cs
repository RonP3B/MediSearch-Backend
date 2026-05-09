using FluentValidation;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;
using MediSearch.Core.Domain.Users;
using MediSearch.Core.Domain.Users.ValueObjects;

namespace MediSearch.Core.Application.Users.Commands.RegisterUser;

public sealed class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private readonly IUserRepository _userRepository;

    public RegisterUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(v => v).ValidValueObject(cmd => FullName.TryFrom(cmd.FirstName, cmd.LastName));

        RuleFor(v => v.Password)
            .NotEmpty()
            .DependentRules(() => RuleFor(v => v.Password).Password());

        RuleFor(v => v.PhoneNumber).ValidValueObject(PhoneNumber.TryFrom);

        RuleFor(v => v)
            .ValidValueObject(cmd => Location.TryFrom(cmd.Province, cmd.Municipality, cmd.Address));

        RuleFor(v => v.ProfileImageFile)
            .NotEmpty()
            .DependentRules(() => RuleFor(v => v.ProfileImageFile).ImageFile().WithMaxSizeMB(5));

        RuleFor(v => v.Username)
            .ValidValueObject(Username.TryFrom)
            .DependentRules(() => RuleFor(v => v.Username).UniqueUsername(_userRepository));

        RuleFor(v => v.Email)
            .ValidValueObject(Email.TryFrom)
            .DependentRules(() => RuleFor(v => v.Email).UniqueEmail(_userRepository));
    }
}
