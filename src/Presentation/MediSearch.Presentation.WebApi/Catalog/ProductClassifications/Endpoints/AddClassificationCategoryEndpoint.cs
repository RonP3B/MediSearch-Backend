using MediSearch.Core.Application.Catalog.ProductClassifications.Commands.AddClassificationCategory;
using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;

namespace MediSearch.Presentation.WebApi.Catalog.ProductClassifications.Endpoints;

internal sealed class AddClassificationCategoryEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(
            AddClassificationCategory,
            "{productClassificationId:guid}/categories"
        );
    }

    [EndpointSummary("Add Product Classification Category")]
    [EndpointDescription("Adds a category to a product classification.")]
    public static async Task<Created<ClassificationCategoryDto>> AddClassificationCategory(
        Guid productClassificationId,
        AddClassificationCategoryRequest request,
        ISender sender,
        LinkGenerator linkGenerator,
        HttpContext httpContext
    )
    {
        var createdCategory = await sender.Send(
            new AddClassificationCategoryCommand(productClassificationId, request.CategoryName)
        );

        return TypedResults.Created(
            linkGenerator.GetUriByName(
                httpContext,
                nameof(
                    GetProductClassificationCategoriesEndpoint.GetProductClassificationCategories
                ),
                new { productClassificationId }
            ),
            createdCategory
        );
    }
}

internal sealed record AddClassificationCategoryRequest
{
    public required string CategoryName { get; init; }
}
