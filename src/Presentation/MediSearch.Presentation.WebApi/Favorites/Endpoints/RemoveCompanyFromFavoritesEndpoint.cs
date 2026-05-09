using MediSearch.Core.Application.Favorites.Commands.RemoveCompanyFromFavorites;

namespace MediSearch.Presentation.WebApi.Favorites.Endpoints;

internal sealed class RemoveCompanyFromFavoritesEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapDelete(RemoveCompanyFromFavorites, "companies/{companyId:guid}");
    }

    [EndpointSummary("Remove Company From Favorites")]
    [EndpointDescription("Removes a company from the authenticated agent's favorites.")]
    public static async Task<NoContent> RemoveCompanyFromFavorites(Guid companyId, ISender sender)
    {
        await sender.Send(new RemoveCompanyFromFavoritesCommand(companyId));
        return TypedResults.NoContent();
    }
}
