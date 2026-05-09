using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Catalog.Products.Queries.GetProductPreviewsWithSeller;

namespace MediSearch.Presentation.WebApi.Catalog.Products.Endpoints;

internal sealed class GetProductPreviewsWithSellerEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetProductPreviewsWithSeller);
    }

    [EndpointSummary("Get Product Previews With Seller")]
    [EndpointDescription(
        "Gets product previews with company seller information. Can filter by company type."
    )]
    public static async Task<
        Ok<IReadOnlyList<ProductPreviewWithSellerDto>>
    > GetProductPreviewsWithSeller(
        [AsParameters] GetProductPreviewsWithSellerRequest request,
        ISender sender
    )
    {
        var result = await sender.Send(new GetProductPreviewsWithSellerQuery(request.CompanyType));
        return TypedResults.Ok(result);
    }
}

internal sealed record GetProductPreviewsWithSellerRequest
{
    public int? CompanyType { get; init; } = null;
}
