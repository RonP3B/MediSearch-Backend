using MediSearch.Core.Application.Catalog.Comments.DTOs;
using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;
using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Catalog.Products.Ports;
using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Infrastructure.Persistence.Catalog.Queries.Sql;

namespace MediSearch.Infrastructure.Persistence.Catalog.Queries.Services;

internal sealed class ProductQueryService(IDbConnectionFactory db) : IProductQueryService
{
    private readonly IDbConnectionFactory _db = db;

    public async Task<IReadOnlyList<ProductPreviewDto>> GetProductPreviewsByCompanyIdAsync(
        Guid companyId,
        Guid? favoriterId,
        int? favoriterTypeId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var products = await conn.QueryAsync<ProductPreviewDto>(
            ProductSql.GetProductPreviewsByCompanyId,
            new
            {
                CompanyId = companyId,
                FavoriterId = favoriterId,
                FavoriterTypeId = favoriterTypeId,
            }
        );

        return [.. products];
    }

    public async Task<IReadOnlyList<ProductPreviewWithSellerDto>> GetProductPreviewsWithSellerAsync(
        int? companyType,
        Guid? favoriterId,
        int? favoriterTypeId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        var products = await conn.QueryAsync<ProductPreviewWithSellerDto>(
            ProductSql.GetProductPreviewsWithSeller,
            new
            {
                CompanyType = companyType,
                FavoriterId = favoriterId,
                FavoriterTypeId = favoriterTypeId,
            }
        );

        return [.. products];
    }

    public async Task<ProductDetailsDto?> GetProductDetailsByIdAsync(
        Guid productId,
        CancellationToken cancellationToken
    )
    {
        await using var conn = await _db.OpenConnectionAsync();

        await using var multi = await conn.QueryMultipleAsync(
            ProductSql.GetProductDetailsById,
            new { ProductId = productId }
        );

        var product = await multi.ReadSingleOrDefaultAsync<ProductDto>();

        if (product is null)
        {
            return null;
        }

        return product.Adapt<ProductDetailsDto>() with
        {
            Classification = await multi.ReadSingleAsync<ProductClassificationDto>(),
            Categories = [.. await multi.ReadAsync<ClassificationCategoryDto>()],
            Company = await multi.ReadSingleAsync<CompanySummaryDto>(),
            Comments =
            [
                .. multi.Read<CommentDto, AuthorDto, AuthorCompanyDto, CommentDto>(
                    (comment, author, authorCompany) =>
                        comment with
                        {
                            Author = author with { Company = authorCompany },
                        },
                    splitOn: $"{nameof(CommentDto.Id)},{nameof(AuthorDto.Id)}"
                ),
            ],
        };
    }
}
