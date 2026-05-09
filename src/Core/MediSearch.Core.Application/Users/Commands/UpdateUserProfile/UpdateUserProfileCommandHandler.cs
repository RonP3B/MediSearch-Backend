using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;
using MediSearch.Core.Domain.Users;
using MediSearch.Core.Domain.Users.ValueObjects;

namespace MediSearch.Core.Application.Users.Commands.UpdateUserProfile;

public sealed class UpdateUserProfileCommandHandler(
    IUserRepository userRepository,
    IFileStorageService fileStorageService,
    IEventBus eventBus,
    ICompensationManager compensationManager,
    ICurrentUser currentUser
) : ICommandHandler<UpdateUserProfileCommand, UserDto>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IFileStorageService _fileStorageService = fileStorageService;
    private readonly IEventBus _eventBus = eventBus;
    private readonly ICompensationManager _compensationManager = compensationManager;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<UserDto> Handle(
        UpdateUserProfileCommand cmd,
        CancellationToken cancellationToken
    )
    {
        Guid authenticatedUserId = _currentUser.GetAuthenticatedUserId();

        var user = await _userRepository.GetByIdOrDefaultAsync(
            EntityId<User>.From(authenticatedUserId),
            cancellationToken
        );

        if (user == null)
        {
            throw NotFoundException.Entity(nameof(User), nameof(User.Id), authenticatedUserId);
        }

        AssetKey? profileImageKey = user.ProfileImageKey;

        if (cmd.ProfileImageFile is not null)
        {
            string newProfileImageKey = await _compensationManager.ExecuteAsync(
                new SaveAssetCompensableOperation(
                    fileDto: cmd.ProfileImageFile,
                    fileStorageService: _fileStorageService,
                    eventBus: _eventBus
                ),
                cancellationToken
            );

            user.UpdateProfileImage(AssetKey.From(newProfileImageKey));
        }

        user.UpdateProfileDetails(
            FullName.From(cmd.FirstName, cmd.LastName),
            Location.From(cmd.Province, cmd.Municipality, cmd.Address),
            PhoneNumber.From(cmd.PhoneNumber)
        );

        return user.Adapt<UserDto>();
    }
}
