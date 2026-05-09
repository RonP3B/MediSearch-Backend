using MediSearch.Core.Application.Favorites.DTOs;
using MediSearch.Core.Application.Favorites.Ports;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Favorites.Queries.GetAllFavorites;

public sealed class GetAllFavoritesQueryHandler(
    IFavoriteQueryService favoriteQueryService,
    ICurrentUser currentUser
) : IQueryHandler<GetAllFavoritesQuery, FavoritesDto>
{
    private readonly IFavoriteQueryService _favoriteQueryService = favoriteQueryService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<FavoritesDto> Handle(
        GetAllFavoritesQuery query,
        CancellationToken cancellationToken
    )
    {
        Agent currentAgent = _currentUser.ToAgent();

        return await _favoriteQueryService.GetAllFavoritesAsync(
            currentAgent.AgentId,
            currentAgent.AgentTypeId,
            cancellationToken
        );
    }
}
