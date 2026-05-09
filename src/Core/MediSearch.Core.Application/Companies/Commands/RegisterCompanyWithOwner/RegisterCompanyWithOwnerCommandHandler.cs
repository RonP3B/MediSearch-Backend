using MediSearch.Core.Application.Accounts.DTOs;
using MediSearch.Core.Application.Accounts.Ports;
using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.Companies.ValueObjects;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;
using MediSearch.Core.Domain.Users;
using MediSearch.Core.Domain.Users.ValueObjects;

namespace MediSearch.Core.Application.Companies.Commands.RegisterCompanyWithOwner;

public sealed class RegisterCompanyWithOwnerCommandHandler(
    IAccountManager accountManager,
    ICompanyRepository companyRepository,
    IUserRepository userRepository,
    IFileStorageService fileStorageService,
    IEventBus eventBus,
    ICompensationManager compensationManager
) : ICommandHandler<RegisterCompanyWithOwnerCommand, CompanyDto>
{
    private readonly IAccountManager _accountManager = accountManager;
    private readonly ICompanyRepository _companyRepository = companyRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IFileStorageService _fileStorageService = fileStorageService;
    private readonly IEventBus _eventBus = eventBus;
    private readonly ICompensationManager _compensationManager = compensationManager;

    public async Task<CompanyDto> Handle(
        RegisterCompanyWithOwnerCommand cmd,
        CancellationToken cancellationToken
    )
    {
        string companyImageKey = await _compensationManager.ExecuteAsync(
            new SaveAssetCompensableOperation(
                fileDto: cmd.Company.ImageFile,
                fileStorageService: _fileStorageService,
                eventBus: _eventBus
            ),
            cancellationToken
        );

        Company company = Company.Create(
            name: CompanyName.From(cmd.Company.Name),
            ceoName: CompanyCeoName.From(cmd.Company.CeoName),
            imageKey: AssetKey.From(companyImageKey),
            email: Email.From(cmd.Company.Email),
            phoneNumber: PhoneNumber.From(cmd.Company.PhoneNumber),
            companyType: CompanyType.GetById(cmd.Company.TypeId),
            location: Location.From(
                cmd.Company.Province,
                cmd.Company.Municipality,
                cmd.Company.Address
            ),
            website: !string.IsNullOrEmpty(cmd.Company.Website)
                ? Url.From(cmd.Company.Website)
                : null,
            facebook: !string.IsNullOrEmpty(cmd.Company.Facebook)
                ? Url.From(cmd.Company.Facebook)
                : null,
            instagram: !string.IsNullOrEmpty(cmd.Company.Instagram)
                ? Url.From(cmd.Company.Instagram)
                : null,
            twitter: !string.IsNullOrEmpty(cmd.Company.Twitter)
                ? Url.From(cmd.Company.Twitter)
                : null
        );

        _companyRepository.Add(company);

        string companyOwnerProfileImageKey = await _compensationManager.ExecuteAsync(
            new SaveAssetCompensableOperation(
                fileDto: cmd.Owner.ProfileImageFile,
                fileStorageService: _fileStorageService,
                eventBus: _eventBus
            ),
            cancellationToken
        );

        var registeredExternalUserDto = await _compensationManager.ExecuteAsync(
            new RegisterExternalUserCompensableOperation(
                registerExternalUserDto: cmd.Owner.Adapt<RegisterExternalUserDto>(),
                accountManager: _accountManager,
                eventBus: _eventBus
            ),
            cancellationToken
        );

        User companyOwner = User.Create(
            externalId: ExternalId.From(registeredExternalUserDto.Id),
            fullName: FullName.From(cmd.Owner.FirstName, cmd.Owner.LastName),
            username: Username.From(cmd.Owner.Username),
            email: Email.From(cmd.Owner.Email),
            phoneNumber: PhoneNumber.From(cmd.Owner.PhoneNumber),
            roles: [Role.CompanyOwner],
            profileImageKey: AssetKey.From(companyOwnerProfileImageKey),
            location: Location.From(cmd.Owner.Province, cmd.Owner.Municipality, cmd.Owner.Address),
            companyId: company.Id
        );

        _userRepository.Add(companyOwner);

        return company.Adapt<CompanyDto>();
    }
}
