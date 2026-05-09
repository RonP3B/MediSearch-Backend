using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Core.Application.Favorites.Ports;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Favorites.Queries.GetCompanyFavorites;

public sealed class GetCompanyFavoritesQueryHandler(
    IFavoriteQueryService favoriteQueryService,
    ICurrentUser currentUser
) : IQueryHandler<GetCompanyFavoritesQuery, IReadOnlyList<CompanySummaryDto>>
{
    private readonly IFavoriteQueryService _favoriteQueryService = favoriteQueryService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<IReadOnlyList<CompanySummaryDto>> Handle(
        GetCompanyFavoritesQuery query,
        CancellationToken cancellationToken
    )
    {
        Agent currentAgent = _currentUser.ToAgent();

        return await _favoriteQueryService.GetCompanyFavoritesAsync(
            currentAgent.AgentId,
            currentAgent.AgentTypeId,
            cancellationToken
        );
    }
}
