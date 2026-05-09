using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Core.Application.Users.Commands.RegisterUser;

public sealed record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Username,
    string Password,
    string PhoneNumber,
    string Email,
    string Province,
    string Municipality,
    string Address,
    FileDto ProfileImageFile
) : ICommand<UserDto>;
