using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Catalog.Products.Queries.GetProductById;

namespace MediSearch.Presentation.WebApi.Catalog.Products.Endpoints;

internal sealed class GetProductByIdEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetProductById, "{productId:guid}");
    }

    [EndpointSummary("Get Product By Id")]
    [EndpointDescription("Gets a product by id.")]
    public static async Task<Ok<ProductDto>> GetProductById(Guid productId, ISender sender)
    {
        var result = await sender.Send(new GetProductByIdQuery(productId));
        return TypedResults.Ok(result);
    }
}
