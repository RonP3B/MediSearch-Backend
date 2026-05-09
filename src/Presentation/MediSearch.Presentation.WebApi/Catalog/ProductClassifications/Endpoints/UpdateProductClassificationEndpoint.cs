using MediSearch.Core.Application.Catalog.ProductClassifications.Commands.UpdateProductClassification;
using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;

namespace MediSearch.Presentation.WebApi.Catalog.ProductClassifications.Endpoints;

internal sealed class UpdateProductClassificationEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPatch(UpdateProductClassification, "{productClassificationId:guid}");
    }

    [EndpointSummary("Update Product Classification")]
    [EndpointDescription("Updates a product classification.")]
    public static async Task<Ok<ProductClassificationDto>> UpdateProductClassification(
        Guid productClassificationId,
        UpdateProductClassificationRequest request,
        ISender sender
    )
    {
        var updatedClassification = await sender.Send(
            new UpdateProductClassificationCommand(productClassificationId, request.Name)
        );

        return TypedResults.Ok(updatedClassification);
    }
}

internal sealed record UpdateProductClassificationRequest
{
    public required string Name { get; init; }
}
