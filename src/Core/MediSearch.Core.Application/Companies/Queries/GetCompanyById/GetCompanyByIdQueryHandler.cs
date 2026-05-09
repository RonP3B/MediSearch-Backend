using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.SharedKernel.ValueObjects;

namespace MediSearch.Core.Application.Companies.Queries.GetCompanyById;

public sealed class GetCompanyByIdQueryHandler(ICompanyRepository companyRepository)
    : IQueryHandler<GetCompanyByIdQuery, CompanyDto>
{
    private readonly ICompanyRepository _companyRepository = companyRepository;

    public async Task<CompanyDto> Handle(
        GetCompanyByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var company = await _companyRepository.GetByIdOrDefaultAsync(
            EntityId<Company>.From(query.CompanyId),
            cancellationToken
        );

        if (company is null)
        {
            throw NotFoundException.Entity(nameof(Company), nameof(Company.Id), query.CompanyId);
        }

        return company.Adapt<CompanyDto>();
    }
}
