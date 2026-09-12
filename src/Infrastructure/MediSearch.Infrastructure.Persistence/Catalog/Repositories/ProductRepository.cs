using MediSearch.Core.Domain.Catalog.Products;
using MediSearch.Core.Domain.Catalog.Products.ValueObjects;
using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Infrastructure.Persistence.Catalog.Repositories;

internal sealed class ProductRepository(AppDbContext dbContext) : IProductRepository
{
    public async Task<Product?> GetByIdOrDefaultAsync(
        EntityId<Product> id,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .Products.Include(p => p.CategoryIds)
            .Include(p => p.ImageKeys)
            .SingleOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        EntityId<Product> id,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.Products.AnyAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsWithNameAsync(
        EntityId<Company> companyId,
        ProductName name,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.Products.AnyAsync(
            p => p.CompanyId == companyId && p.Name.Normalized == name.Normalized,
            cancellationToken
        );
    }

    public async Task<bool> ExistsWithNameAsync(
        EntityId<Company> companyId,
        ProductName name,
        EntityId<Product> excludeId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.Products.AnyAsync(
            p => p.CompanyId == companyId
                && p.Name.Normalized == name.Normalized
                && p.Id != excludeId,
            cancellationToken
        );
    }

    public async Task<bool> BelongsToCompanyTypeAsync(
        EntityId<Product> productId,
        CompanyType companyType,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .Products.Where(p => p.Id == productId)
            .Join(dbContext.Companies, p => p.CompanyId, c => c.Id, (p, c) => c.CompanyTypeId)
            .AnyAsync(typeId => typeId == companyType.Id, cancellationToken);
    }

    public void Add(Product product)
    {
        dbContext.Products.Add(product);
    }

    public void Remove(Product product)
    {
        dbContext.Products.Remove(product);
    }
}
