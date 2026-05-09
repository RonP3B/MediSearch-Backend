using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;
using MediSearch.Core.Application.Catalog.ProductClassifications.Queries.GetProductClassificationCategories;

namespace MediSearch.Presentation.WebApi.Catalog.ProductClassifications.Endpoints;

internal sealed class GetProductClassificationCategoriesEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(
            GetProductClassificationCategories,
            "{productClassificationId:guid}/categories"
        );
    }

    [EndpointSummary("Get Product Classification Categories")]
    [EndpointDescription("Gets all categories that belong to a classification.")]
    public static async Task<
        Ok<IReadOnlyList<ClassificationCategoryDto>>
    > GetProductClassificationCategories(Guid productClassificationId, ISender sender)
    {
        var result = await sender.Send(
            new GetProductClassificationCategoriesQuery(productClassificationId)
        );
        return TypedResults.Ok(result);
    }
}
