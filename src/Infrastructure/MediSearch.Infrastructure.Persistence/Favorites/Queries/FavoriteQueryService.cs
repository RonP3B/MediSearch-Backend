using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Core.Application.Favorites.DTOs;
using MediSearch.Core.Application.Favorites.Ports;
using MediSearch.Core.Application.Users.DTOs;
using MediSearch.Core.Domain.SharedKernel.Enums;
using MediSearch.Infrastructure.Persistence.Shared.Exceptions;

namespace MediSearch.Infrastructure.Persistence.Favorites.Queries;

internal sealed class FavoriteQueryService(IDbConnectionFactory db) : IFavoriteQueryService
{
    private readonly IDbConnectionFactory _db = db;

    public async Task<CompanyFavoritedByDto> GetCompanyFavoritedByAsync(
        Guid companyId,
        Guid favoriterAgentId,
        int favoriterAgentTypeId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var result = await conn.QuerySingleOrDefaultAsync<CompanyFavoritedByDto>(
            FavoriteSql.GetCompanyFavoritedBy,
            new
            {
                CompanyId = companyId,
                FavoriterAgentId = favoriterAgentId,
                FavoriterAgentTypeId = favoriterAgentTypeId,
                UserAgentTypeId = AgentType.User.Id,
                CompanyAgentTypeId = AgentType.Company.Id,
            }
        );

        if (result is null)
        {
            throw new ExpectedResultNotFoundException(
                $"Company favorite projection for company '{companyId}'"
                    + $" and agent '{favoriterAgentId}' was expected to exist but was not found."
            );
        }

        return result;
    }

    public async Task<ProductFavoritedByDto> GetProductFavoritedByAsync(
        Guid productId,
        Guid favoriterAgentId,
        int favoriterAgentTypeId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var result = await conn.QuerySingleOrDefaultAsync<ProductFavoritedByDto>(
            FavoriteSql.GetProductFavoritedBy,
            new
            {
                ProductId = productId,
                FavoriterAgentId = favoriterAgentId,
                FavoriterAgentTypeId = favoriterAgentTypeId,
                UserAgentTypeId = AgentType.User.Id,
                CompanyAgentTypeId = AgentType.Company.Id,
            }
        );

        if (result is null)
        {
            throw new ExpectedResultNotFoundException(
                $"Product favorite projection for product '{productId}'"
                    + $" and agent '{favoriterAgentId}' was expected to exist but was not found."
            );
        }

        return result;
    }

    public async Task<IReadOnlyList<UserContactInfoDto>> GetCompanyFavoritersContactInfoAsync(
        Guid companyId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var favoritersContactInfo = await conn.QueryAsync<UserContactInfoDto>(
            FavoriteSql.GetCompanyFavoritersContactInfo,
            new
            {
                CompanyId = companyId,
                UserAgentTypeId = AgentType.User.Id,
                CompanyAgentTypeId = AgentType.Company.Id,
            }
        );

        return [.. favoritersContactInfo];
    }

    public async Task<IReadOnlyList<UserContactInfoDto>> GetProductFavoritersContactInfoAsync(
        Guid productId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var favoritersContactInfo = await conn.QueryAsync<UserContactInfoDto>(
            FavoriteSql.GetProductFavoritersContactInfo,
            new
            {
                ProductId = productId,
                UserAgentTypeId = AgentType.User.Id,
                CompanyAgentTypeId = AgentType.Company.Id,
            }
        );

        return [.. favoritersContactInfo];
    }

    public async Task<FavoritesDto> GetAllFavoritesAsync(
        Guid favoriterId,
        int favoriterTypeId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        await using var multi = await conn.QueryMultipleAsync(
            FavoriteSql.GetAllFavorites,
            new { FavoriterId = favoriterId, FavoriterTypeId = favoriterTypeId }
        );

        return new FavoritesDto
        {
            FavoriteProducts = [.. await multi.ReadAsync<ProductPreviewDto>()],
            FavoriteCompanies = [.. await multi.ReadAsync<CompanySummaryDto>()],
        };
    }

    public async Task<IReadOnlyList<CompanySummaryDto>> GetCompanyFavoritesAsync(
        Guid favoriterId,
        int favoriterTypeId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var companies = await conn.QueryAsync<CompanySummaryDto>(
            FavoriteSql.GetCompanyFavorites,
            new { FavoriterId = favoriterId, FavoriterTypeId = favoriterTypeId }
        );

        return [.. companies];
    }

    public async Task<IReadOnlyList<ProductPreviewDto>> GetProductFavoritesAsync(
        Guid favoriterId,
        int favoriterTypeId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var products = await conn.QueryAsync<ProductPreviewDto>(
            FavoriteSql.GetProductFavorites,
            new { FavoriterId = favoriterId, FavoriterTypeId = favoriterTypeId }
        );

        return [.. products];
    }
}
