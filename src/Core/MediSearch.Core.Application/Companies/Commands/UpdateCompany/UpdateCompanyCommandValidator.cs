using FluentValidation;
using MediSearch.Core.Application.Companies.Extensions;
using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.Companies.ValueObjects;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Companies.Commands.UpdateCompany;

public sealed class UpdateCompanyCommandValidator : AbstractValidator<UpdateCompanyCommand>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly ICurrentUser _currentUser;

    public UpdateCompanyCommandValidator(
        ICompanyRepository companyRepository,
        ICurrentUser currentUser
    )
    {
        _companyRepository = companyRepository;
        _currentUser = currentUser;

        RuleFor(v => v.CompanyId).ValidValueObject(EntityId<Company>.TryFrom);

        RuleFor(v => v.Name)
            .ValidValueObject(CompanyName.TryFrom)
            .DependentRules(() =>
                RuleFor(v => v)
                    .UniqueCompanyName(
                        _companyRepository,
                        _currentUser,
                        nameSelector: cmd => cmd.Name
                    )
                    .OverridePropertyName(nameof(UpdateCompanyCommand.Name))
            );

        RuleFor(v => v.CeoName).ValidValueObject(CompanyCeoName.TryFrom);

        RuleFor(v => v.Email).ValidValueObject(Email.TryFrom);

        RuleFor(v => v.PhoneNumber).ValidValueObject(PhoneNumber.TryFrom);

        RuleFor(v => v)
            .ValidValueObject(cmd => Location.TryFrom(cmd.Province, cmd.Municipality, cmd.Address));

        RuleFor(v => v.ImageFile).ImageFile().WithMaxSizeMB(5).When(v => v.ImageFile != null);

        RuleFor(v => v.Website)
            .ValidValueObject(Url.TryFrom)
            .When(v => !string.IsNullOrEmpty(v.Website));

        RuleFor(v => v.Facebook)
            .ValidValueObject(Url.TryFrom)
            .When(v => !string.IsNullOrEmpty(v.Facebook));

        RuleFor(v => v.Twitter)
            .ValidValueObject(Url.TryFrom)
            .When(v => !string.IsNullOrEmpty(v.Twitter));

        RuleFor(v => v.Instagram)
            .ValidValueObject(Url.TryFrom)
            .When(v => !string.IsNullOrEmpty(v.Instagram));
    }
}
