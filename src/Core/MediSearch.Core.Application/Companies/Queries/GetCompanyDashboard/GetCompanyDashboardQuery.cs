using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Core.Application.Companies.Queries.GetCompanyDashboard;

[Authorize(Permission = PermissionCodes.GetCompanyDashboard)]
public sealed record GetCompanyDashboardQuery(
    Guid CompanyId,
    int TopProvinces,
    int TopProducts,
    int TopClassifications,
    int TopProductInteractions,
    int TopFavoriteProducts
) : IQuery<CompanyDashboardDto>;
