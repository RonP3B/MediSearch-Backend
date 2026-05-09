using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Ports;

public interface IProductClassificationQueryService : IQueryService
{
    Task<IReadOnlyList<ProductClassificationDto>> GetProductClassificationsAsync(
        CancellationToken cancellationToken
    );

    Task<IReadOnlyList<ClassificationCategoryDto>> GetProductClassificationCategoriesAsync(
        Guid classificationId,
        CancellationToken cancellationToken
    );
}
