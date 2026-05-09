using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Core.Application.Companies.Ports;

namespace MediSearch.Core.Application.Companies.Queries.LookupCompaniesByName;

public sealed class LookupCompaniesByNameQueryHandler(
    ICompanyQueryService companyQueryService,
    ICurrentUser currentUser
) : IQueryHandler<LookupCompaniesByNameQuery, IReadOnlyList<CompanyLookupDto>>
{
    private readonly ICompanyQueryService _companyQueryService = companyQueryService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<IReadOnlyList<CompanyLookupDto>> Handle(
        LookupCompaniesByNameQuery query,
        CancellationToken cancellationToken
    )
    {
        return await _companyQueryService.LookupCompaniesByNameAsync(
            query.CompanyName,
            _currentUser.CompanyId ?? Guid.Empty,
            cancellationToken
        );
    }
}
