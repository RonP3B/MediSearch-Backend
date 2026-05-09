using MediSearch.Core.Application.Catalog.Products.Commands.UpdateProduct;
using MediSearch.Core.Application.Catalog.Products.DTOs;

namespace MediSearch.Presentation.WebApi.Catalog.Products.Endpoints;

internal sealed class UpdateProductEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPut(UpdateProduct, "{productId:guid}").DisableAntiforgery();
    }

    [EndpointSummary("Update Product")]
    [EndpointDescription("Updates an existing product.")]
    public static async Task<Ok<ProductDto>> UpdateProduct(
        Guid productId,
        [FromForm] UpdateProductRequest request,
        ISender sender
    )
    {
        var updatedProduct = await sender.Send(
            request.Adapt<UpdateProductCommand>() with
            {
                Id = productId,
            }
        );

        return TypedResults.Ok(updatedProduct);
    }
}

internal sealed record UpdateProductRequest
{
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required Guid ClassificationId { get; init; }
    public required IReadOnlyList<Guid> CategoryIds { get; init; }
    public required double PriceAmount { get; init; }
    public required string PriceCurrency { get; init; }
    public required int Quantity { get; init; }
    public IReadOnlyList<IFormFile> ImageFiles { get; init; } = [];
}
