using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Catalog.Products.Queries.GetProductPreviewsByCompanyId;

namespace MediSearch.Presentation.WebApi.Companies.Endpoints;

internal sealed class GetCompanyProductPreviewsEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetProductPreviewsByCompanyId, "{companyId:guid}/products");
    }

    [EndpointSummary("Get Product Previews By Company Id")]
    [EndpointDescription("Gets product previews for a company.")]
    public static async Task<Ok<IReadOnlyList<ProductPreviewDto>>> GetProductPreviewsByCompanyId(
        Guid companyId,
        ISender sender
    )
    {
        var result = await sender.Send(new GetProductPreviewsByCompanyIdQuery(companyId));
        return TypedResults.Ok(result);
    }
}
