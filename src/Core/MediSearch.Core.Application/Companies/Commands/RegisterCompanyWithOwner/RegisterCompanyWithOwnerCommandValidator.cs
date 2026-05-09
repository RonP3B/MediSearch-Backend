using FluentValidation;
using MediSearch.Core.Application.Companies.Constants;
using MediSearch.Core.Application.Companies.Extensions;
using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.Companies.ValueObjects;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;
using MediSearch.Core.Domain.Users;
using MediSearch.Core.Domain.Users.ValueObjects;

namespace MediSearch.Core.Application.Companies.Commands.RegisterCompanyWithOwner;

public sealed class RegisterCompanyWithOwnerCommandValidator
    : AbstractValidator<RegisterCompanyWithOwnerCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ICompanyRepository _companyRepository;

    private static readonly HashSet<int> CompanyTypeIds =
    [
        .. CompanyType.List().Select(type => type.Id),
    ];

    public RegisterCompanyWithOwnerCommandValidator(
        IUserRepository userRepository,
        ICompanyRepository companyRepository
    )
    {
        _userRepository = userRepository;
        _companyRepository = companyRepository;

        AddCompanyOwnerIdentityRules();
        AddCompanyOwnerContactRules();
        AddCompanyOwnerLocationRules();
        AddCompanyOwnerMediaRules();

        AddCompanyIdentityRules();
        AddCompanyContactRules();
        AddCompanyLocationRules();
        AddCompanyMediaRules();
        AddCompanyTypeRules();
    }

    private void AddCompanyOwnerIdentityRules()
    {
        RuleFor(v => v)
            .ValidValueObject(
                cmd => FullName.TryFrom(cmd.Owner.FirstName, cmd.Owner.LastName),
                prefix: CompanyPrefixes.CompanyOwner
            );

        RuleFor(v => v.Owner.Username)
            .ValidValueObject(Username.TryFrom)
            .DependentRules(() => RuleFor(v => v.Owner.Username).UniqueUsername(_userRepository));

        RuleFor(v => v.Owner.Password)
            .NotEmpty()
            .DependentRules(() => RuleFor(v => v.Owner.Password).Password());
    }

    private void AddCompanyOwnerContactRules()
    {
        RuleFor(v => v.Owner.Email)
            .ValidValueObject(Email.TryFrom)
            .DependentRules(() => RuleFor(v => v.Owner.Email).UniqueEmail(_userRepository));

        RuleFor(v => v.Owner.PhoneNumber).ValidValueObject(PhoneNumber.TryFrom);
    }

    private void AddCompanyOwnerLocationRules()
    {
        RuleFor(v => v)
            .ValidValueObject(
                cmd =>
                    Location.TryFrom(cmd.Owner.Province, cmd.Owner.Municipality, cmd.Owner.Address),
                prefix: CompanyPrefixes.CompanyOwner
            );
    }

    private void AddCompanyOwnerMediaRules()
    {
        RuleFor(v => v.Owner.ProfileImageFile)
            .NotEmpty()
            .DependentRules(() =>
                RuleFor(v => v.Owner.ProfileImageFile).ImageFile().WithMaxSizeMB(5)
            );
    }

    private void AddCompanyIdentityRules()
    {
        RuleFor(v => v.Company.Name)
            .ValidValueObject(CompanyName.TryFrom)
            .DependentRules(() =>
            {
                RuleFor(v => v.Company.Name).UniqueCompanyName(_companyRepository);
            });

        RuleFor(v => v.Company.CeoName).ValidValueObject(CompanyCeoName.TryFrom);
    }

    private void AddCompanyContactRules()
    {
        RuleFor(v => v.Company.Email)
            .ValidValueObject(Email.TryFrom)
            .DependentRules(() => RuleFor(v => v.Company.Email).UniqueEmail(_companyRepository));

        RuleFor(v => v.Company.PhoneNumber).ValidValueObject(PhoneNumber.TryFrom);

        RuleFor(v => v.Company.Website)
            .ValidValueObject(Url.TryFrom)
            .When(v => !string.IsNullOrEmpty(v.Company.Website));

        RuleFor(v => v.Company.Facebook)
            .ValidValueObject(Url.TryFrom)
            .When(v => !string.IsNullOrEmpty(v.Company.Facebook));

        RuleFor(v => v.Company.Instagram)
            .ValidValueObject(Url.TryFrom)
            .When(v => !string.IsNullOrEmpty(v.Company.Instagram));

        RuleFor(v => v.Company.Twitter)
            .ValidValueObject(Url.TryFrom)
            .When(v => !string.IsNullOrEmpty(v.Company.Twitter));
    }

    private void AddCompanyLocationRules()
    {
        RuleFor(v => v)
            .ValidValueObject(
                cmd =>
                    Location.TryFrom(
                        cmd.Company.Province,
                        cmd.Company.Municipality,
                        cmd.Company.Address
                    ),
                prefix: CompanyPrefixes.Company
            );
    }

    private void AddCompanyMediaRules()
    {
        RuleFor(v => v.Company.ImageFile)
            .NotEmpty()
            .DependentRules(() => RuleFor(v => v.Company.ImageFile).ImageFile().WithMaxSizeMB(5));
    }

    private void AddCompanyTypeRules()
    {
        RuleFor(v => v.Company.TypeId)
            .Must(CompanyTypeIds.Contains)
            .WithCustomErrorCode(ApplicationErrorCodes.InvalidCompanyType);
    }
}
