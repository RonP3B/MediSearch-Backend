using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;
using MediSearch.Core.Application.Catalog.ProductClassifications.Ports;
using MediSearch.Infrastructure.Persistence.Catalog.Queries.Sql;

namespace MediSearch.Infrastructure.Persistence.Catalog.Queries.Services;

internal sealed class ProductClassificationQueryService(IDbConnectionFactory db)
    : IProductClassificationQueryService
{
    private readonly IDbConnectionFactory _db = db;

    public async Task<IReadOnlyList<ProductClassificationDto>> GetProductClassificationsAsync(
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var classifications = await conn.QueryAsync<ProductClassificationDto>(
            ProductClassificationSql.GetProductClassifications
        );

        return [.. classifications];
    }

    public async Task<IReadOnlyList<ClassificationCategoryDto>> GetProductClassificationCategoriesAsync(
        Guid classificationId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var categories = await conn.QueryAsync<ClassificationCategoryDto>(
            ProductClassificationSql.GetProductClassificationCategories,
            new { ClassificationId = classificationId }
        );

        return [.. categories];
    }
}
