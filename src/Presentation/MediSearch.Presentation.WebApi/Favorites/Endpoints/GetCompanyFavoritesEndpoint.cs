using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Core.Application.Favorites.Queries.GetCompanyFavorites;

namespace MediSearch.Presentation.WebApi.Favorites.Endpoints;

internal sealed class GetCompanyFavoritesEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetCompanyFavorites, "companies");
    }

    [EndpointSummary("Get Company Favorites")]
    [EndpointDescription("Gets all the authenticated agent's favorite companies.")]
    public static async Task<Ok<IReadOnlyList<CompanySummaryDto>>> GetCompanyFavorites(
        ISender sender
    )
    {
        var result = await sender.Send(new GetCompanyFavoritesQuery());
        return TypedResults.Ok(result);
    }
}
