using MediSearch.Core.Application.Catalog.ProductClassifications.Commands.DeleteProductClassification;

namespace MediSearch.Presentation.WebApi.Catalog.ProductClassifications.Endpoints;

internal sealed class DeleteProductClassificationEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapDelete(DeleteProductClassification, "{productClassificationId:guid}");
    }

    [EndpointSummary("Delete Product Classification")]
    [EndpointDescription("Deletes a product classification by id.")]
    public static async Task<NoContent> DeleteProductClassification(
        Guid productClassificationId,
        ISender sender
    )
    {
        await sender.Send(new DeleteProductClassificationCommand(productClassificationId));
        return TypedResults.NoContent();
    }
}
