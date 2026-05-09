using MediSearch.Core.Application.Catalog.Products.Commands.DeleteProduct;

namespace MediSearch.Presentation.WebApi.Catalog.Products.Endpoints;

internal sealed class DeleteProductEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapDelete(DeleteProduct, "{productId:guid}");
    }

    [EndpointSummary("Delete Product")]
    [EndpointDescription("Deletes a product by id.")]
    public static async Task<NoContent> DeleteProduct(Guid productId, ISender sender)
    {
        await sender.Send(new DeleteProductCommand(productId));
        return TypedResults.NoContent();
    }
}
