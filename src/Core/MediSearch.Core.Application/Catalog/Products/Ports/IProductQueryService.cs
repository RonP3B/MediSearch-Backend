using MediSearch.Core.Application.Catalog.Products.DTOs;

namespace MediSearch.Core.Application.Catalog.Products.Ports;

public interface IProductQueryService : IQueryService
{
    Task<IReadOnlyList<ProductPreviewDto>> GetProductPreviewsByCompanyIdAsync(
        Guid companyId,
        Guid? favoriterId,
        int? favoriterTypeId,
        CancellationToken cancellationToken
    );

    Task<IReadOnlyList<ProductPreviewWithSellerDto>> GetProductPreviewsWithSellerAsync(
        int? companyType,
        Guid? favoriterId,
        int? favoriterTypeId,
        CancellationToken cancellationToken
    );

    Task<ProductDetailsDto?> GetProductDetailsByIdAsync(
        Guid productId,
        CancellationToken cancellationToken
    );
}
