using MediSearch.Core.Domain.Catalog.Products;
using MediSearch.Core.Domain.Favorites.ProductFavorites;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Infrastructure.Persistence.Favorites.Repositories;

internal sealed class ProductFavoriteRepository(AppDbContext dbContext) : IProductFavoriteRepository
{
    public async Task<ProductFavorite?> GetByProductAndAgentOrDefaultAsync(
        EntityId<Product> productId,
        Agent favoriterAgent,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.ProductFavorites.SingleOrDefaultAsync(
            pf =>
                pf.ProductId == productId
                && pf.FavoriterAgent.AgentTypeId == favoriterAgent.AgentTypeId
                && pf.FavoriterAgent.AgentId == favoriterAgent.AgentId,
            cancellationToken
        );
    }

    public void Add(ProductFavorite productFavorite)
    {
        dbContext.ProductFavorites.Add(productFavorite);
    }

    public void Remove(ProductFavorite productFavorite)
    {
        dbContext.ProductFavorites.Remove(productFavorite);
    }
}
