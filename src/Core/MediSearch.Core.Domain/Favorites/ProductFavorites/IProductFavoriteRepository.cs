using MediSearch.Core.Domain.Catalog.Products;

namespace MediSearch.Core.Domain.Favorites.ProductFavorites;

public interface IProductFavoriteRepository : IRepository
{
    Task<ProductFavorite?> GetByProductAndAgentOrDefaultAsync(
        EntityId<Product> productId,
        Agent favoriterAgent,
        CancellationToken cancellationToken = default
    );
    void Add(ProductFavorite productFavorite);
    void Remove(ProductFavorite productFavorite);
}
