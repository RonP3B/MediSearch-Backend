namespace MediSearch.Core.Domain.Favorites.CompanyFavorites;

public sealed class CompanyAddedToFavoritesDomainEvent(
    Guid companyFavoriteId,
    Guid companyId,
    Guid favoriterAgentId,
    int favoriterAgentTypeId
) : DomainEvent
{
    public Guid CompanyFavoriteId { get; } = companyFavoriteId;
    public Guid CompanyId { get; } = companyId;
    public Guid FavoriterAgentId { get; } = favoriterAgentId;
    public int FavoriterAgentTypeId { get; } = favoriterAgentTypeId;
}
