using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Favorites.Ports;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Favorites.Queries.GetProductFavorites;

public sealed class GetProductFavoritesQueryHandler(
    IFavoriteQueryService favoriteQueryService,
    ICurrentUser currentUser
) : IQueryHandler<GetProductFavoritesQuery, IReadOnlyList<ProductPreviewDto>>
{
    private readonly IFavoriteQueryService _favoriteQueryService = favoriteQueryService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<IReadOnlyList<ProductPreviewDto>> Handle(
        GetProductFavoritesQuery query,
        CancellationToken cancellationToken
    )
    {
        Agent currentAgent = _currentUser.ToAgent();

        return await _favoriteQueryService.GetProductFavoritesAsync(
            currentAgent.AgentId,
            currentAgent.AgentTypeId,
            cancellationToken
        );
    }
}
