using FluentValidation;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;
using MediSearch.Core.Domain.Users.ValueObjects;

namespace MediSearch.Core.Application.Users.Commands.UpdateUserProfile;

public sealed class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(v => v).ValidValueObject(cmd => FullName.TryFrom(cmd.FirstName, cmd.LastName));

        RuleFor(v => v)
            .ValidValueObject(cmd => Location.TryFrom(cmd.Province, cmd.Municipality, cmd.Address));

        RuleFor(v => v.PhoneNumber).ValidValueObject(PhoneNumber.TryFrom);

        RuleFor(v => v.ProfileImageFile)
            .NotEmpty()
            .DependentRules(() => RuleFor(v => v.ProfileImageFile).ImageFile().WithMaxSizeMB(5))
            .When(v => v.ProfileImageFile != null);
    }
}
