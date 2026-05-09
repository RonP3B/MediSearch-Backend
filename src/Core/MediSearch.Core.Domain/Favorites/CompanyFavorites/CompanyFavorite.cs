using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.SharedKernel.Enums;

namespace MediSearch.Core.Domain.Favorites.CompanyFavorites;

public sealed class CompanyFavorite : BaseAuditableEntity
{
    private CompanyFavorite() { }

    public EntityId<CompanyFavorite> Id { get; private set; } = EntityId<CompanyFavorite>.Empty;
    public Agent FavoriterAgent { get; private set; } = Agent.Empty;
    public EntityId<Company> CompanyId { get; private set; } = EntityId<Company>.Empty;

    public static CompanyFavorite Create(Agent favoriter, EntityId<Company> companyId)
    {
        if (favoriter.AgentTypeId == AgentType.Company.Id && favoriter.AgentId == companyId)
        {
            throw new BusinessRuleException(
                nameof(CompanyFavorite),
                CompanyFavoriteErrorCodes.CompanyCannotFavoriteItself
            );
        }

        var companyFavoriteId = EntityId<CompanyFavorite>.New();

        var favorite = new CompanyFavorite
        {
            Id = companyFavoriteId,
            FavoriterAgent = favoriter,
            CompanyId = companyId,
        };

        favorite.AddDomainEvent(
            new CompanyAddedToFavoritesDomainEvent(
                companyFavoriteId,
                companyId,
                favoriter.AgentId,
                favoriter.AgentTypeId
            )
        );

        return favorite;
    }
}
