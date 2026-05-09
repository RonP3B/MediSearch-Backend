using MediSearch.Core.Domain.Companies.ValueObjects;

namespace MediSearch.Core.Domain.Companies;

public interface ICompanyRepository : IRepository
{
    Task<Company?> GetByIdOrDefaultAsync(
        EntityId<Company> id,
        CancellationToken cancellationToken = default
    );
    Task<bool> ExistsAsync(EntityId<Company> id, CancellationToken cancellationToken = default);
    Task<bool> IsCompanyNameTakenAsync(
        CompanyName companyName,
        CancellationToken cancellationToken
    );
    Task<bool> IsCompanyNameTakenAsync(
        CompanyName companyName,
        EntityId<Company> excludeId,
        CancellationToken cancellationToken
    );
    Task<bool> AreCompaniesOfDifferentTypeAsync(
        EntityId<Company> companyAId,
        EntityId<Company> companyBId,
        CancellationToken cancellationToken = default
    );
    Task<bool> IsEmailTakenAsync(Email email, CancellationToken cancellationToken = default);
    Task<CompanyType> GetCompanyTypeAsync(
        EntityId<Company> companyId,
        CancellationToken cancellationToken = default
    );
    void Add(Company company);
}
