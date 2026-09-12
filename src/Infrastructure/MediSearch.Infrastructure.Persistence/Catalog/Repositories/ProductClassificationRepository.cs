using MediSearch.Core.Domain.Catalog.ProductClassifications;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Infrastructure.Persistence.Catalog.Repositories;

internal sealed class ProductClassificationRepository(AppDbContext dbContext)
    : IProductClassificationRepository
{
    public async Task<ProductClassification?> GetByIdOrDefaultAsync(
        EntityId<ProductClassification> id,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .ProductClassifications.Include(pc => pc.Categories)
            .SingleOrDefaultAsync(pc => pc.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        EntityId<ProductClassification> id,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.ProductClassifications.AnyAsync(
            pc => pc.Id == id,
            cancellationToken
        );
    }

    public async Task<bool> ExistsByNameAsync(
        Name name,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.ProductClassifications.AnyAsync(
            pc => pc.Name.Normalized == name.Normalized,
            cancellationToken
        );
    }

    public async Task<bool> ExistsByNameAsync(
        Name name,
        EntityId<ProductClassification> excludeId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.ProductClassifications.AnyAsync(
            pc => pc.Name.Normalized == name.Normalized && pc.Id != excludeId,
            cancellationToken
        );
    }

    public async Task<bool> CategoriesBelongToClassificationAsync(
        EntityId<ProductClassification> classificationId,
        IReadOnlyCollection<EntityId<ClassificationCategory>> categoryIds,
        CancellationToken cancellationToken = default
    )
    {
        var count = await dbContext
            .ProductClassifications.Where(pc => pc.Id == classificationId)
            .SelectMany(pc => pc.Categories)
            .CountAsync(c => categoryIds.Contains(c.Id), cancellationToken);

        return count == categoryIds.Count;
    }

    public void Add(ProductClassification productClassification)
    {
        dbContext.ProductClassifications.Add(productClassification);
    }

    public void Remove(ProductClassification productClassification)
    {
        dbContext.ProductClassifications.Remove(productClassification);
    }
}
