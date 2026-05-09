using MediSearch.Core.Domain.Companies;

namespace MediSearch.Core.Domain.Favorites.CompanyFavorites;

public interface ICompanyFavoriteRepository : IRepository
{
    Task<CompanyFavorite?> GetByCompanyAndAgentOrDefaultAsync(
        EntityId<Company> companyId,
        Agent favoriterAgent,
        CancellationToken cancellationToken = default
    );
    void Add(CompanyFavorite companyFavorite);
    void Remove(CompanyFavorite companyFavorite);
}
