using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Core.Application.Home.DTOs;
using MediSearch.Core.Application.Home.Ports;
using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.SharedKernel.Enums;

namespace MediSearch.Infrastructure.Persistence.Home.Queries;

internal sealed class HomeQueryService(IDbConnectionFactory db) : IHomeQueryService
{
    private readonly IDbConnectionFactory _db = db;

    public async Task<PublicHomeDto> GetPublicHomeAsync(CancellationToken cancellationToken)
    {
        await using var conn = await _db.OpenConnectionAsync();

        await using var multi = await conn.QueryMultipleAsync(
            HomeSql.GetPublicHome,
            new
            {
                PharmacyCompanyTypeId = CompanyType.Pharmacy.Id,
                LaboratoryCompanyTypeId = CompanyType.Laboratory.Id,
            }
        );

        return new PublicHomeDto
        {
            LastestPharmacies = [.. await multi.ReadAsync<CompanySummaryDto>()],
            LastestLaboratories = [.. await multi.ReadAsync<CompanySummaryDto>()],
            LastestLaboratoryProducts = [.. await multi.ReadAsync<ProductPreviewDto>()],
            LastestPharmacyProducts = [.. await multi.ReadAsync<ProductPreviewDto>()],
        };
    }

    public async Task<ClientHomeDto> GetClientHomeAsync(
        Guid userId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        await using var multi = await conn.QueryMultipleAsync(
            HomeSql.GetClientHome,
            new
            {
                UserId = userId,
                UserAgentTypeId = AgentType.User.Id,
                PharmacyCompanyTypeId = CompanyType.Pharmacy.Id,
            }
        );

        return new ClientHomeDto
        {
            LastestProducts = [.. await multi.ReadAsync<ProductPreviewDto>()],
            PharmaciesInYourProvince = [.. await multi.ReadAsync<CompanySummaryDto>()],
            LatestFavoritedProducts = [.. await multi.ReadAsync<ProductPreviewDto>()],
            LatestFavoritedCompanies = [.. await multi.ReadAsync<CompanySummaryDto>()],
        };
    }
}
