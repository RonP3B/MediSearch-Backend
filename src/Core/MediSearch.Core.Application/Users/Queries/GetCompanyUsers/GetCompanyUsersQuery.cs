using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Core.Application.Users.Queries.GetCompanyUsers;

[Authorize]
public sealed record GetCompanyUsersQuery(Guid CompanyId) : IQuery<IReadOnlyList<UserDto>>;
