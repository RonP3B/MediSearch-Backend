using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Catalog.Products.Queries.GetProductDetailsById;

namespace MediSearch.Presentation.WebApi.Catalog.Products.Endpoints;

internal sealed class GetProductDetailsByIdEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetProductDetailsById, "{productId:guid}/details");
    }

    [EndpointSummary("Get Product Details By Id")]
    [EndpointDescription("Gets a product with details by id.")]
    public static async Task<Ok<ProductDetailsDto>> GetProductDetailsById(
        Guid productId,
        ISender sender
    )
    {
        var result = await sender.Send(new GetProductDetailsByIdQuery(productId));
        return TypedResults.Ok(result);
    }
}
