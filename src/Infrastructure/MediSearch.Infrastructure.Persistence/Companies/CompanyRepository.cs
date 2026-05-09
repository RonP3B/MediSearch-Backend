using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.Companies.ValueObjects;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Infrastructure.Persistence.Companies;

internal sealed class CompanyRepository(AppDbContext dbContext) : ICompanyRepository
{
    public async Task<Company?> GetByIdOrDefaultAsync(
        EntityId<Company> id,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.Companies.SingleOrDefaultAsync(
            company => company.Id == id,
            cancellationToken
        );
    }

    public async Task<bool> ExistsAsync(
        EntityId<Company> id,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.Companies.AnyAsync(company => company.Id == id, cancellationToken);
    }

    public async Task<bool> IsCompanyNameTakenAsync(
        CompanyName companyName,
        CancellationToken cancellationToken
    )
    {
        return await dbContext.Companies.AnyAsync(
            company => company.Name == companyName,
            cancellationToken
        );
    }

    public async Task<bool> IsCompanyNameTakenAsync(
        CompanyName companyName,
        EntityId<Company> excludeId,
        CancellationToken cancellationToken
    )
    {
        return await dbContext.Companies.AnyAsync(
            company => company.Name == companyName && company.Id != excludeId,
            cancellationToken
        );
    }

    public async Task<bool> IsEmailTakenAsync(
        Email email,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.Companies.AnyAsync(c => c.Email == email, cancellationToken);
    }

    public async Task<bool> AreCompaniesOfDifferentTypeAsync(
        EntityId<Company> companyAId,
        EntityId<Company> companyBId,
        CancellationToken cancellationToken = default
    )
    {
        if (companyAId == companyBId)
        {
            return false;
        }

        var types = await dbContext
            .Companies.Where(c => c.Id == companyAId || c.Id == companyBId)
            .Select(c => c.CompanyTypeId)
            .Distinct()
            .ToListAsync(cancellationToken);

        return types.Count == 2;
    }

    public async Task<CompanyType> GetCompanyTypeAsync(
        EntityId<Company> companyId,
        CancellationToken cancellationToken = default
    )
    {
        var typeId = await dbContext
            .Companies.Where(c => c.Id == companyId)
            .Select(c => c.CompanyTypeId)
            .FirstOrDefaultAsync(cancellationToken);

        return CompanyType.GetById(typeId);
    }

    public void Add(Company company)
    {
        dbContext.Companies.Add(company);
    }
}
