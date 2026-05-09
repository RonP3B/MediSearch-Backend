using MediSearch.Core.Application.Home.DTOs;
using MediSearch.Core.Application.Home.Ports;

namespace MediSearch.Core.Application.Home.Queries.GetPublicHome;

public sealed class GetPublicHomeQueryHandler(IHomeQueryService homeQueryService)
    : IQueryHandler<GetPublicHomeQuery, PublicHomeDto>
{
    private readonly IHomeQueryService _homeQueryService = homeQueryService;

    public async Task<PublicHomeDto> Handle(
        GetPublicHomeQuery query,
        CancellationToken cancellationToken
    )
    {
        return await _homeQueryService.GetPublicHomeAsync(cancellationToken);
    }
}
