using MediSearch.Core.Application.Accounts.DTOs;
using MediSearch.Core.Application.Accounts.Ports;
using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;
using MediSearch.Core.Domain.Users;
using MediSearch.Core.Domain.Users.ValueObjects;

namespace MediSearch.Core.Application.Users.Commands.RegisterCompanyUser;

public sealed class RegisterCompanyUserCommandHandler(
    IAccountManager accountManager,
    IUserRepository userRepository,
    IEventBus eventBus,
    ICompensationManager compensationManager,
    ICurrentUser currentUser
) : ICommandHandler<RegisterCompanyUserCommand, UserDto>
{
    private readonly IAccountManager _accountManager = accountManager;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEventBus _eventBus = eventBus;
    private readonly ICompensationManager _compensationManager = compensationManager;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<UserDto> Handle(
        RegisterCompanyUserCommand cmd,
        CancellationToken cancellationToken
    )
    {
        Guid companyId = _currentUser.GetAuthenticatedUserCompanyId();

        string username = CompanyUserCredentialsGenerator.GenerateUsername(
            cmd.FirstName,
            cmd.LastName
        );

        var registeredExternalUserDto = await _compensationManager.ExecuteAsync(
            new RegisterExternalUserCompensableOperation(
                new RegisterExternalUserDto
                {
                    Username = username,
                    Password = CompanyUserCredentialsGenerator.GenerateTempPassword(),
                    Email = cmd.Email,
                    PhoneNumber = cmd.PhoneNumber,
                },
                accountManager: _accountManager,
                eventBus: _eventBus
            ),
            cancellationToken
        );

        User user = User.Create(
            externalId: ExternalId.From(registeredExternalUserDto.Id),
            fullName: FullName.From(cmd.FirstName, cmd.LastName),
            username: Username.From(username),
            email: Email.From(cmd.Email),
            phoneNumber: PhoneNumber.From(cmd.PhoneNumber),
            location: Location.From(cmd.Province, cmd.Municipality, cmd.Address),
            roles: [Role.GetById(cmd.RoleInCompany)],
            companyId: EntityId<Company>.From(companyId)
        );

        _userRepository.Add(user);

        return user.Adapt<UserDto>();
    }
}
