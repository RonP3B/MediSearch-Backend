using MediSearch.Core.Domain.Catalog.Products.ValueObjects;
using MediSearch.Core.Domain.Companies;

namespace MediSearch.Core.Domain.Catalog.Products;

public interface IProductRepository : IRepository
{
    Task<Product?> GetByIdOrDefaultAsync(
        EntityId<Product> id,
        CancellationToken cancellationToken = default
    );
    Task<bool> ExistsAsync(EntityId<Product> id, CancellationToken cancellationToken = default);
    Task<bool> ExistsWithNameAsync(
        EntityId<Company> companyId,
        ProductName name,
        CancellationToken cancellationToken = default
    );
    Task<bool> ExistsWithNameAsync(
        EntityId<Company> companyId,
        ProductName name,
        EntityId<Product> excludeId,
        CancellationToken cancellationToken = default
    );
    Task<bool> BelongsToCompanyTypeAsync(
        EntityId<Product> productId,
        CompanyType companyType,
        CancellationToken cancellationToken = default
    );
    void Add(Product product);
    void Remove(Product product);
}
