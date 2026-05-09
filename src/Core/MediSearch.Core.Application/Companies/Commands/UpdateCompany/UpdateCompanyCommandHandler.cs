using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.Companies.ValueObjects;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Companies.Commands.UpdateCompany;

public sealed class UpdateCompanyCommandHandler(
    ICompanyRepository companyRepository,
    IFileStorageService fileStorageService,
    ICompensationManager compensationManager,
    IEventBus eventBus,
    ICurrentUser currentUser
) : ICommandHandler<UpdateCompanyCommand, CompanyDto>
{
    private readonly ICompanyRepository _companyRepository = companyRepository;
    private readonly IFileStorageService _fileStorageService = fileStorageService;
    private readonly ICompensationManager _compensationManager = compensationManager;
    private readonly IEventBus _eventBus = eventBus;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<CompanyDto> Handle(
        UpdateCompanyCommand cmd,
        CancellationToken cancellationToken
    )
    {
        Guid authenticatedUserCompanyId = _currentUser.GetAuthenticatedUserCompanyId();

        if (authenticatedUserCompanyId != cmd.CompanyId)
        {
            throw new ForbiddenAccessException();
        }

        var company = await _companyRepository.GetByIdOrDefaultAsync(
            EntityId<Company>.From(cmd.CompanyId),
            cancellationToken
        );

        if (company == null)
        {
            throw NotFoundException.Entity(nameof(Company), nameof(Company.Id), cmd.CompanyId);
        }

        AssetKey? imageKey = company.ImageKey;

        if (cmd.ImageFile is not null)
        {
            string newImageKey = await _compensationManager.ExecuteAsync(
                new SaveAssetCompensableOperation(
                    fileDto: cmd.ImageFile,
                    fileStorageService: _fileStorageService,
                    eventBus: _eventBus
                ),
                cancellationToken
            );

            imageKey = AssetKey.From(newImageKey);
        }

        company.UpdateDetails(
            name: CompanyName.From(cmd.Name),
            ceoName: CompanyCeoName.From(cmd.CeoName),
            location: Location.From(cmd.Province, cmd.Municipality, cmd.Address),
            email: Email.From(cmd.Email),
            phoneNumber: PhoneNumber.From(cmd.PhoneNumber),
            imageKey: AssetKey.From(imageKey),
            website: !string.IsNullOrEmpty(cmd.Website) ? Url.From(cmd.Website) : null,
            facebook: !string.IsNullOrEmpty(cmd.Facebook) ? Url.From(cmd.Facebook) : null,
            instagram: !string.IsNullOrEmpty(cmd.Instagram) ? Url.From(cmd.Instagram) : null,
            twitter: !string.IsNullOrEmpty(cmd.Twitter) ? Url.From(cmd.Twitter) : null
        );

        return company.Adapt<CompanyDto>();
    }
}
