using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Core.Application.Companies.Queries.GetCompanySummaries;

namespace MediSearch.Presentation.WebApi.Companies.Endpoints;

internal sealed class GetCompanySummariesEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetCompanySummaries);
    }

    [EndpointSummary("Get Company Summaries")]
    [EndpointDescription("Gets the list of companies.")]
    public static async Task<Ok<IReadOnlyList<CompanySummaryDto>>> GetCompanySummaries(
        [AsParameters] GetCompanySummariesRequest request,
        ISender sender
    )
    {
        var result = await sender.Send(new GetCompanySummariesQuery(request.CompanyType));
        return TypedResults.Ok(result);
    }
}

internal sealed record GetCompanySummariesRequest
{
    public int? CompanyType { get; init; } = null;
}
