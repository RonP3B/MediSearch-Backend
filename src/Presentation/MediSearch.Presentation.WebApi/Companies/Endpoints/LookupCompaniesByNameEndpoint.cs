using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Core.Application.Companies.Queries.LookupCompaniesByName;

namespace MediSearch.Presentation.WebApi.Companies.Endpoints;

internal sealed class LookupCompaniesByNameEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(LookupCompaniesByName, "search");
    }

    [EndpointSummary("Lookup Companies By Name")]
    [EndpointDescription("Looks up companies by name prefix.")]
    public static async Task<Ok<IReadOnlyList<CompanyLookupDto>>> LookupCompaniesByName(
        [AsParameters] LookupCompaniesByNameRequest request,
        ISender sender
    )
    {
        var result = await sender.Send(new LookupCompaniesByNameQuery(request.CompanyName));
        return TypedResults.Ok(result);
    }
}

internal sealed record LookupCompaniesByNameRequest
{
    public required string CompanyName { get; init; }
}
