using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Core.Application.Companies.Queries.GetCompanyDetailsById;

namespace MediSearch.Presentation.WebApi.Companies.Endpoints;

internal sealed class GetCompanyDetailsByIdEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetCompanyDetailsById, "{companyId:guid}/details");
    }

    [EndpointSummary("Get Company Details By Id")]
    [EndpointDescription("Gets a company with its product previews by id.")]
    public static async Task<Ok<CompanyDetailsDto>> GetCompanyDetailsById(
        Guid companyId,
        ISender sender
    )
    {
        var result = await sender.Send(new GetCompanyDetailsByIdQuery(companyId));
        return TypedResults.Ok(result);
    }
}
