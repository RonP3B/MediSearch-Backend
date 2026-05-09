using MediSearch.Core.Domain.Catalog.Products;

namespace MediSearch.Core.Domain.Favorites.ProductFavorites;

public sealed class ProductFavorite : BaseAuditableEntity
{
    private ProductFavorite() { }

    public EntityId<ProductFavorite> Id { get; private set; } = EntityId<ProductFavorite>.Empty;
    public Agent FavoriterAgent { get; private set; } = Agent.Empty;
    public EntityId<Product> ProductId { get; private set; } = EntityId<Product>.Empty;

    public static ProductFavorite Create(Agent favoriter, EntityId<Product> productId)
    {
        var productFavoriteId = EntityId<ProductFavorite>.New();

        var favorite = new ProductFavorite
        {
            Id = productFavoriteId,
            FavoriterAgent = favoriter,
            ProductId = productId,
        };

        favorite.AddDomainEvent(
            new ProductAddedToFavoritesDomainEvent(
                productFavoriteId,
                productId,
                favoriter.AgentId,
                favoriter.AgentTypeId
            )
        );

        return favorite;
    }
}
