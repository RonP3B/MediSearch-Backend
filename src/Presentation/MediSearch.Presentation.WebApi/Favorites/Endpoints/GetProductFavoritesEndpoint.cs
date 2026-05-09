using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Favorites.Queries.GetProductFavorites;

namespace MediSearch.Presentation.WebApi.Favorites.Endpoints;

internal sealed class GetProductFavoritesEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetProductFavorites, "products");
    }

    [EndpointSummary("Get Product Favorites")]
    [EndpointDescription("Gets all the authenticated agent's favorite products.")]
    public static async Task<Ok<IReadOnlyList<ProductPreviewDto>>> GetProductFavorites(
        ISender sender
    )
    {
        var result = await sender.Send(new GetProductFavoritesQuery());
        return TypedResults.Ok(result);
    }
}
