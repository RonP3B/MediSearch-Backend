using MediSearch.Core.Application.Favorites.Commands.AddCompanyToFavorites;

namespace MediSearch.Presentation.WebApi.Favorites.Endpoints;

internal sealed class AddCompanyToFavoritesEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(AddCompanyToFavorites, "companies");
    }

    [EndpointSummary("Add Company To Favorites")]
    [EndpointDescription("Adds a company to the authenticated agent's favorites.")]
    public static async Task<NoContent> AddCompanyToFavorites(
        AddCompanyToFavoritesRequest request,
        ISender sender
    )
    {
        await sender.Send(new AddCompanyToFavoritesCommand(request.CompanyId));
        return TypedResults.NoContent();
    }
}

internal sealed record AddCompanyToFavoritesRequest
{
    public required Guid CompanyId { get; init; }
}
