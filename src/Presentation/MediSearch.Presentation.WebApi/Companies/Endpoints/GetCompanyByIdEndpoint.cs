using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Core.Application.Companies.Queries.GetCompanyById;

namespace MediSearch.Presentation.WebApi.Companies.Endpoints;

internal sealed class GetCompanyByIdEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetCompanyById, "{companyId:guid}");
    }

    [EndpointSummary("Get Company By Id")]
    [EndpointDescription("Gets a company by id.")]
    public static async Task<Ok<CompanyDto>> GetCompanyById(Guid companyId, ISender sender)
    {
        var result = await sender.Send(new GetCompanyByIdQuery(companyId));
        return TypedResults.Ok(result);
    }
}
