using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.Favorites.CompanyFavorites;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Infrastructure.Persistence.Favorites.Repositories;

internal sealed class CompanyFavoriteRepository(AppDbContext dbContext) : ICompanyFavoriteRepository
{
    public async Task<CompanyFavorite?> GetByCompanyAndAgentOrDefaultAsync(
        EntityId<Company> companyId,
        Agent favoriterAgent,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.CompanyFavorites.SingleOrDefaultAsync(
            cf =>
                cf.CompanyId == companyId
                && cf.FavoriterAgent.AgentTypeId == favoriterAgent.AgentTypeId
                && cf.FavoriterAgent.AgentId == favoriterAgent.AgentId,
            cancellationToken
        );
    }

    public void Add(CompanyFavorite companyFavorite)
    {
        dbContext.CompanyFavorites.Add(companyFavorite);
    }

    public void Remove(CompanyFavorite companyFavorite)
    {
        dbContext.CompanyFavorites.Remove(companyFavorite);
    }
}
