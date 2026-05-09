using MediSearch.Core.Application.Home.DTOs;
using MediSearch.Core.Application.Home.Ports;

namespace MediSearch.Core.Application.Home.Queries.GetClientHome;

public sealed class GetClientHomeQueryHandler(
    IHomeQueryService homeQueryService,
    ICurrentUser currentUser
) : IQueryHandler<GetClientHomeQuery, ClientHomeDto>
{
    private readonly IHomeQueryService _homeQueryService = homeQueryService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<ClientHomeDto> Handle(
        GetClientHomeQuery query,
        CancellationToken cancellationToken
    )
    {
        return await _homeQueryService.GetClientHomeAsync(
            _currentUser.GetAuthenticatedUserId(),
            cancellationToken
        );
    }
}
