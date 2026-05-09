using MediSearch.Core.Application.Favorites.DTOs;
using MediSearch.Core.Application.Favorites.Queries.GetAllFavorites;

namespace MediSearch.Presentation.WebApi.Favorites.Endpoints;

internal sealed class GetAllFavoritesEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetAllFavorites);
    }

    [EndpointSummary("Get All Favorites")]
    [EndpointDescription("Gets all the authenticated agent's favorites.")]
    public static async Task<Ok<FavoritesDto>> GetAllFavorites(ISender sender)
    {
        var result = await sender.Send(new GetAllFavoritesQuery());
        return TypedResults.Ok(result);
    }
}
