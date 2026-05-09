namespace MediSearch.Core.Domain.Catalog.ProductClassifications;

public interface IProductClassificationRepository : IRepository
{
    Task<ProductClassification?> GetByIdOrDefaultAsync(
        EntityId<ProductClassification> id,
        CancellationToken cancellationToken = default
    );
    Task<bool> CategoriesBelongToClassificationAsync(
        EntityId<ProductClassification> classificationId,
        IReadOnlyCollection<EntityId<ClassificationCategory>> categoryIds,
        CancellationToken cancellationToken = default
    );
    Task<bool> ExistsAsync(
        EntityId<ProductClassification> id,
        CancellationToken cancellationToken = default
    );
    Task<bool> ExistsByNameAsync(Name name, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(
        Name name,
        EntityId<ProductClassification> excludeId,
        CancellationToken cancellationToken = default
    );
    void Add(ProductClassification productClassification);
    void Remove(ProductClassification productClassification);
}
