using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Core.Application.Users.Commands.RegisterCompanyUser;

[Authorize(Permission = PermissionCodes.AddCompanyUser)]
public sealed record RegisterCompanyUserCommand(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    int RoleInCompany,
    string Province,
    string Municipality,
    string Address
) : ICommand<UserDto>;
