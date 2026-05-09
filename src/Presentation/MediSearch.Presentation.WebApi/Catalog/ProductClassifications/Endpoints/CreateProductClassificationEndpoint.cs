using MediSearch.Core.Application.Catalog.ProductClassifications.Commands.CreateProductClassification;
using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;

namespace MediSearch.Presentation.WebApi.Catalog.ProductClassifications.Endpoints;

internal sealed class CreateProductClassificationEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(CreateProductClassification);
    }

    [EndpointSummary("Create Product Classification")]
    [EndpointDescription("Creates a new product classification.")]
    public static async Task<Created<ProductClassificationDto>> CreateProductClassification(
        CreateProductClassificationRequest request,
        ISender sender,
        LinkGenerator linkGenerator,
        HttpContext httpContext
    )
    {
        var createdClassification = await sender.Send(
            new CreateProductClassificationCommand(request.Name)
        );

        return TypedResults.Created(
            linkGenerator.GetUriByName(
                httpContext,
                nameof(GetProductClassificationsEndpoint.GetProductClassifications)
            ),
            createdClassification
        );
    }
}

internal sealed record CreateProductClassificationRequest
{
    public required string Name { get; init; }
}
