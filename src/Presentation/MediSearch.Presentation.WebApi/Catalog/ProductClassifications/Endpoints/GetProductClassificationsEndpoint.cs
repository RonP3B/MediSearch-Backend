using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;
using MediSearch.Core.Application.Catalog.ProductClassifications.Queries.GetProductClassifications;

namespace MediSearch.Presentation.WebApi.Catalog.ProductClassifications.Endpoints;

internal sealed class GetProductClassificationsEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetProductClassifications);
    }

    [EndpointSummary("Get Product Classifications")]
    [EndpointDescription("Gets all product classifications.")]
    public static async Task<Ok<IReadOnlyList<ProductClassificationDto>>> GetProductClassifications(
        ISender sender
    )
    {
        var result = await sender.Send(new GetProductClassificationsQuery());
        return TypedResults.Ok(result);
    }
}
