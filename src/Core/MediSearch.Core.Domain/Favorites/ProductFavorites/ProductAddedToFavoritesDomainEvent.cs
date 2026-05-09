namespace MediSearch.Core.Domain.Favorites.ProductFavorites;

public sealed class ProductAddedToFavoritesDomainEvent(
    Guid productFavoriteId,
    Guid productId,
    Guid favoriterAgentId,
    int favoriterAgentTypeId
) : DomainEvent
{
    public Guid ProductFavoriteId { get; } = productFavoriteId;
    public Guid ProductId { get; } = productId;
    public Guid FavoriterAgentId { get; } = favoriterAgentId;
    public int FavoriterAgentTypeId { get; } = favoriterAgentTypeId;
}
