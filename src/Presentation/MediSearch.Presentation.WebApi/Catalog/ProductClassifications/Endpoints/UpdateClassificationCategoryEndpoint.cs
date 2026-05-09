using MediSearch.Core.Application.Catalog.ProductClassifications.Commands.UpdateClassificationCategory;
using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;

namespace MediSearch.Presentation.WebApi.Catalog.ProductClassifications.Endpoints;

internal sealed class UpdateClassificationCategoryEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPatch(
            UpdateClassificationCategory,
            "{productClassificationId:guid}/categories/{classificationCategoryId:guid}"
        );
    }

    [EndpointSummary("Update Classification Category")]
    [EndpointDescription("Updates a category inside a classification.")]
    public static async Task<Ok<ClassificationCategoryDto>> UpdateClassificationCategory(
        Guid productClassificationId,
        Guid classificationCategoryId,
        UpdateClassificationCategoryRequest request,
        ISender sender
    )
    {
        var updatedCategory = await sender.Send(
            new UpdateClassificationCategoryCommand(
                productClassificationId,
                classificationCategoryId,
                request.CategoryName
            )
        );

        return TypedResults.Ok(updatedCategory);
    }
}

internal sealed record UpdateClassificationCategoryRequest
{
    public required string CategoryName { get; init; }
}
