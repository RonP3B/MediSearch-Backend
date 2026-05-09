using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Core.Application.Users.Commands.UpdateUserProfile;

[Authorize]
public sealed record UpdateUserProfileCommand(
    string FirstName,
    string LastName,
    string Province,
    string Municipality,
    string Address,
    string PhoneNumber,
    FileDto? ProfileImageFile = null
) : ICommand<UserDto>;
