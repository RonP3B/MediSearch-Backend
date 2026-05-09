using MediSearch.Core.Application.Favorites.Commands.AddProductToFavorites;

namespace MediSearch.Presentation.WebApi.Favorites.Endpoints;

internal sealed class AddProductToFavoritesEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(AddProductToFavorites, "products");
    }

    [EndpointSummary("Add Product To Favorites")]
    [EndpointDescription("Adds a product to the authenticated agent's favorites.")]
    public static async Task<NoContent> AddProductToFavorites(
        AddProductToFavoritesRequest request,
        ISender sender
    )
    {
        await sender.Send(new AddProductToFavoritesCommand(request.ProductId));
        return TypedResults.NoContent();
    }
}

internal sealed record AddProductToFavoritesRequest
{
    public required Guid ProductId { get; init; }
}
