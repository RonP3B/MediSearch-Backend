using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Core.Application.Companies.Queries.GetCompanyDashboard;

namespace MediSearch.Presentation.WebApi.Companies.Endpoints;

internal sealed class GetCompanyDashboardEndpoint : IEndpoint
{
    public void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(GetCompanyDashboard, "{companyId:guid}/dashboard");
    }

    [EndpointSummary("Get Dashboard")]
    [EndpointDescription("Gets dashboard data for the authenticated company.")]
    public static async Task<Ok<CompanyDashboardDto>> GetCompanyDashboard(
        Guid companyId,
        [AsParameters] GetCompanyDashboardRequest request,
        ISender sender
    )
    {
        var result = await sender.Send(
            new GetCompanyDashboardQuery(
                CompanyId: companyId,
                TopProvinces: request.TopProvinces ?? 4,
                TopProducts: request.TopProducts ?? 10,
                TopClassifications: request.TopClassifications ?? 8,
                TopProductInteractions: request.TopProductInteractions ?? 5,
                TopFavoriteProducts: request.TopFavoriteProducts ?? 6
            )
        );

        return TypedResults.Ok(result);
    }
}

internal sealed record GetCompanyDashboardRequest
{
    public int? TopProvinces { get; init; }
    public int? TopProducts { get; init; }
    public int? TopClassifications { get; init; }
    public int? TopProductInteractions { get; init; }
    public int? TopFavoriteProducts { get; init; }
}
