using MediSearch.Core.Application.Home.DTOs;
using MediSearch.Core.Application.Home.Queries.GetPublicHome;

namespace MediSearch.Presentation.WebApi.Home.Endpoints;

internal sealed class GetPublicHomeEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetPublicHome, "public");
    }

    [EndpointSummary("Get Public Home")]
    [EndpointDescription("Gets the public home page data.")]
    public static async Task<Ok<PublicHomeDto>> GetPublicHome(ISender sender)
    {
        var result = await sender.Send(new GetPublicHomeQuery());
        return TypedResults.Ok(result);
    }
}
