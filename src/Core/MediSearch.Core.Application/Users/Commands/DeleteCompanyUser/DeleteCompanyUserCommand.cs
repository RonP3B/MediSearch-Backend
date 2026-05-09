namespace MediSearch.Core.Application.Users.Commands.DeleteCompanyUser;

[Authorize(Permission = PermissionCodes.RemoveCompanyUser)]
public sealed record DeleteCompanyUserCommand(Guid UserId) : ICommand;
