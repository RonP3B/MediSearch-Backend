using MediSearch.Core.Application.Favorites.Commands.RemoveProductFromFavorites;

namespace MediSearch.Presentation.WebApi.Favorites.Endpoints;

internal sealed class RemoveProductFromFavoritesEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapDelete(RemoveProductFromFavorites, "products/{productId:guid}");
    }

    [EndpointSummary("Remove Product From Favorites")]
    [EndpointDescription("Removes a product from the authenticated agent's favorites.")]
    public static async Task<NoContent> RemoveProductFromFavorites(Guid productId, ISender sender)
    {
        await sender.Send(new RemoveProductFromFavoritesCommand(productId));
        return TypedResults.NoContent();
    }
}
