using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Core.Application.Users.Ports;

namespace MediSearch.Core.Application.Users.Queries.GetCompanyUsers;

public sealed class GetCompanyUsersQueryHandler(
    IUserQueryService userQueryService,
    ICurrentUser currentUser
) : IQueryHandler<GetCompanyUsersQuery, IReadOnlyList<UserDto>>
{
    private readonly IUserQueryService _userQueryService = userQueryService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<IReadOnlyList<UserDto>> Handle(
        GetCompanyUsersQuery query,
        CancellationToken cancellationToken
    )
    {
        if (_currentUser.GetAuthenticatedUserCompanyId() != query.CompanyId)
        {
            throw new ForbiddenAccessException();
        }

        return await _userQueryService.GetCompanyUsersAsync(query.CompanyId, cancellationToken);
    }
}
