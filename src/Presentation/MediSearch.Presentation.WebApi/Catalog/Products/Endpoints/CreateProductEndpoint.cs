using MediSearch.Core.Application.Catalog.Products.Commands.CreateProduct;
using MediSearch.Core.Application.Catalog.Products.DTOs;

namespace MediSearch.Presentation.WebApi.Catalog.Products.Endpoints;

internal sealed class CreateProductEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(CreateProduct).DisableAntiforgery();
    }

    [EndpointSummary("Create Product")]
    [EndpointDescription("Creates a new product.")]
    public static async Task<Created<ProductDto>> CreateProduct(
        [FromForm] CreateProductRequest request,
        ISender sender,
        LinkGenerator linkGenerator,
        HttpContext httpContext
    )
    {
        var createdProduct = await sender.Send(request.Adapt<CreateProductCommand>());

        return TypedResults.Created(
            linkGenerator.GetUriByName(
                httpContext,
                nameof(GetProductByIdEndpoint.GetProductById),
                new { productId = createdProduct.Id }
            ),
            createdProduct
        );
    }
}

internal sealed record CreateProductRequest
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required Guid ClassificationId { get; init; }
    public required IReadOnlyList<Guid> CategoryIds { get; init; }
    public required double PriceAmount { get; init; }
    public required string PriceCurrency { get; init; }
    public required int Quantity { get; init; }
    public required IReadOnlyList<IFormFile> ImageFiles { get; init; }
}
