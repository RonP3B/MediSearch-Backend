using MediSearch.Core.Application.Catalog.ProductClassifications.Commands.RemoveClassificationCategory;

namespace MediSearch.Presentation.WebApi.Catalog.ProductClassifications.Endpoints;

internal sealed class RemoveClassificationCategoryEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapDelete(
            RemoveClassificationCategory,
            "{productClassificationId:guid}/categories/{classificationCategoryId:guid}"
        );
    }

    [EndpointSummary("Remove Classification Category")]
    [EndpointDescription("Removes a category from a classification.")]
    public static async Task<NoContent> RemoveClassificationCategory(
        Guid productClassificationId,
        Guid classificationCategoryId,
        ISender sender
    )
    {
        await sender.Send(
            new RemoveClassificationCategoryCommand(
                productClassificationId,
                classificationCategoryId
            )
        );
        return TypedResults.NoContent();
    }
}
