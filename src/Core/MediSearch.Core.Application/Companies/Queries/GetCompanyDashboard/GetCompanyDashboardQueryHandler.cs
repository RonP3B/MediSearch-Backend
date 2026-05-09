using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Core.Application.Companies.Ports;

namespace MediSearch.Core.Application.Companies.Queries.GetCompanyDashboard;

public sealed class GetCompanyDashboardQueryHandler(
    ICompanyQueryService companyQueryService,
    ICurrentUser currentUser
) : IQueryHandler<GetCompanyDashboardQuery, CompanyDashboardDto>
{
    private readonly ICompanyQueryService _companyQueryService = companyQueryService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<CompanyDashboardDto> Handle(
        GetCompanyDashboardQuery query,
        CancellationToken cancellationToken
    )
    {
        if (_currentUser.GetAuthenticatedUserCompanyId() != query.CompanyId)
        {
            throw new ForbiddenAccessException();
        }

        return await _companyQueryService.GetCompanyDashboardAsync(
            query.CompanyId,
            query.TopProvinces,
            query.TopProducts,
            query.TopClassifications,
            query.TopProductInteractions,
            query.TopFavoriteProducts,
            cancellationToken
        );
    }
}
