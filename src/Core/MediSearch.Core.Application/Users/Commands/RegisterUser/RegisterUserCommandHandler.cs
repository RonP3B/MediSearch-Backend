using MediSearch.Core.Application.Accounts.DTOs;
using MediSearch.Core.Application.Accounts.Ports;
using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;
using MediSearch.Core.Domain.Users;
using MediSearch.Core.Domain.Users.ValueObjects;

namespace MediSearch.Core.Application.Users.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler(
    IAccountManager accountManager,
    IUserRepository userRepository,
    IFileStorageService fileStorageService,
    IEventBus eventBus,
    ICompensationManager compensationManager
) : ICommandHandler<RegisterUserCommand, UserDto>
{
    private readonly IAccountManager _accountManager = accountManager;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IFileStorageService _fileStorageService = fileStorageService;
    private readonly IEventBus _eventBus = eventBus;
    private readonly ICompensationManager _compensationManager = compensationManager;

    public async Task<UserDto> Handle(RegisterUserCommand cmd, CancellationToken cancellationToken)
    {
        string profileImageKey = await _compensationManager.ExecuteAsync(
            new SaveAssetCompensableOperation(
                fileDto: cmd.ProfileImageFile,
                fileStorageService: _fileStorageService,
                eventBus: _eventBus
            ),
            cancellationToken
        );

        var registeredExternalUserDto = await _compensationManager.ExecuteAsync(
            new RegisterExternalUserCompensableOperation(
                registerExternalUserDto: cmd.Adapt<RegisterExternalUserDto>(),
                accountManager: _accountManager,
                eventBus: _eventBus
            ),
            cancellationToken
        );

        User user = User.Create(
            externalId: ExternalId.From(registeredExternalUserDto.Id),
            fullName: FullName.From(cmd.FirstName, cmd.LastName),
            username: Username.From(cmd.Username),
            email: Email.From(cmd.Email),
            phoneNumber: PhoneNumber.From(cmd.PhoneNumber),
            location: Location.From(cmd.Province, cmd.Municipality, cmd.Address),
            roles: [Role.Client],
            profileImageKey: AssetKey.From(profileImageKey)
        );

        _userRepository.Add(user);

        return user.Adapt<UserDto>();
    }
}
