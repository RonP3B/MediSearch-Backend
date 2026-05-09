using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Core.Application.Companies.Ports;
using MediSearch.Core.Application.Shared.DTOs;
using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.SharedKernel.Enums;
using MediSearch.Infrastructure.Persistence.Shared.Exceptions;

namespace MediSearch.Infrastructure.Persistence.Companies.Queries;

internal sealed class CompanyQueryService(IDbConnectionFactory db) : ICompanyQueryService
{
    private readonly IDbConnectionFactory _db = db;

    public async Task<IReadOnlyList<CompanySummaryDto>> GetCompanySummariesAsync(
        int? companyType,
        Guid? favoriterId,
        int? favoriterTypeId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var companies = await conn.QueryAsync<CompanySummaryDto>(
            CompanySql.GetCompanySummaries,
            new
            {
                CompanyType = companyType,
                FavoriterId = favoriterId,
                FavoriterTypeId = favoriterTypeId,
            }
        );

        return [.. companies];
    }

    public async Task<IReadOnlyList<CompanyLookupDto>> LookupCompaniesByNameAsync(
        string name,
        Guid userCompanyId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var companies = await conn.QueryAsync<CompanyLookupDto>(
            CompanySql.LookupCompaniesByName,
            new
            {
                Name = $"{name.ToLowerInvariant()}%",
                UserCompanyId = userCompanyId,
                PharmacyTypeId = CompanyType.Pharmacy.Id,
                LaboratoryTypeId = CompanyType.Laboratory.Id,
            }
        );

        return [.. companies];
    }

    public async Task<string> GetCompanyNameAsync(
        Guid companyId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var result = await conn.QuerySingleOrDefaultAsync<string>(
            CompanySql.GetCompanyName,
            new { CompanyId = companyId }
        );

        if (result == null)
        {
            throw new ExpectedResultNotFoundException(
                $"Company with ID '{companyId}' was expected to exist"
                    + $" but was not found when attempting to retrieve its name."
            );
        }

        return result;
    }

    public async Task<CompanyDetailsDto?> GetCompanyDetailsByIdOrDefaultAsync(
        Guid companyId,
        Guid? favoriterId,
        int? favoriterTypeId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        await using var multi = await conn.QueryMultipleAsync(
            CompanySql.GetCompanyDetailsById,
            new
            {
                CompanyId = companyId,
                FavoriterId = favoriterId,
                FavoriterTypeId = favoriterTypeId,
            }
        );

        var company = await multi.ReadSingleOrDefaultAsync<CompanyDto>();

        if (company is null)
        {
            return null;
        }

        return company.Adapt<CompanyDetailsDto>() with
        {
            Products = [.. await multi.ReadAsync<ProductPreviewDto>()],
        };
    }

    public async Task<CompanyDashboardDto> GetCompanyDashboardAsync(
        Guid companyId,
        int topProvinces,
        int topProducts,
        int topClassifications,
        int topProductInteractions,
        int topFavoriteProducts,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        await using var multi = await conn.QueryMultipleAsync(
            CompanySql.GetCompanyDashboard,
            new
            {
                CompanyId = companyId,
                CompanyAgentTypeId = AgentType.Company.Id,
                TopProvinces = topProvinces,
                TopProducts = topProducts,
                TopClassifications = topClassifications,
                TopProductInteractions = topProductInteractions,
                TopFavoriteProducts = topFavoriteProducts,
            }
        );

        bool companyExists = await multi.ReadSingleAsync<bool>();

        if (!companyExists)
        {
            throw new ExpectedResultNotFoundException(
                $"Company with ID '{companyId}' was expected to exist"
                    + $" but was not found when attempting to retrieve its dashboard."
            );
        }

        return new CompanyDashboardDto
        {
            ProductsCount = await multi.ReadSingleOrDefaultAsync<long>(),
            UsersCount = await multi.ReadSingleOrDefaultAsync<long>(),
            ChatsCount = await multi.ReadSingleOrDefaultAsync<long>(),
            OtherTypeCompaniesCount = await multi.ReadSingleOrDefaultAsync<long>(),
            ProvincesWithMostOtherTypeCompanies = [.. await multi.ReadAsync<ItemCountDto>()],
            TopProductsByQuantity = [.. await multi.ReadAsync<ItemCountDto>()],
            TopClassifications = [.. await multi.ReadAsync<ItemCountDto>()],
            TopProductsWithMostInteractions = [.. await multi.ReadAsync<ItemCountDto>()],
            TopProductsWithMostFavorites = [.. await multi.ReadAsync<ItemCountDto>()],
        };
    }
}
