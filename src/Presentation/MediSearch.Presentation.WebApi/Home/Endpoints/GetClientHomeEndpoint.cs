using MediSearch.Core.Application.Home.DTOs;
using MediSearch.Core.Application.Home.Queries.GetClientHome;

namespace MediSearch.Presentation.WebApi.Home.Endpoints;

internal sealed class GetClientHomeEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetClientHome, "client");
    }

    [EndpointSummary("Get Client Home")]
    [EndpointDescription("Gets the personalized home page data for the authenticated user client.")]
    public static async Task<Ok<ClientHomeDto>> GetClientHome(ISender sender)
    {
        var result = await sender.Send(new GetClientHomeQuery());
        return TypedResults.Ok(result);
    }
}
