using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Core.Application.Companies.Ports;

public interface ICompanyQueryService : IQueryService
{
    Task<IReadOnlyList<CompanySummaryDto>> GetCompanySummariesAsync(
        int? companyType,
        Guid? favoriterId,
        int? favoriterTypeId,
        CancellationToken cancellationToken
    );

    Task<IReadOnlyList<CompanyLookupDto>> LookupCompaniesByNameAsync(
        string name,
        Guid userCompanyId,
        CancellationToken cancellationToken
    );

    Task<string> GetCompanyNameAsync(Guid companyId, CancellationToken cancellationToken);

    Task<CompanyDetailsDto?> GetCompanyDetailsByIdOrDefaultAsync(
        Guid companyId,
        Guid? favoriterId,
        int? favoriterTypeId,
        CancellationToken cancellationToken
    );

    Task<CompanyDashboardDto> GetCompanyDashboardAsync(
        Guid companyId,
        int topProvinces,
        int topProducts,
        int topClassifications,
        int topProductInteractions,
        int topFavoriteProducts,
        CancellationToken cancellationToken
    );
}
