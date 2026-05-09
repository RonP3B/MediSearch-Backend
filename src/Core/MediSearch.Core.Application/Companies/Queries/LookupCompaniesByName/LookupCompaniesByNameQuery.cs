using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Core.Application.Companies.Queries.LookupCompaniesByName;

[Authorize]
public sealed record LookupCompaniesByNameQuery(string CompanyName)
    : IQuery<IReadOnlyList<CompanyLookupDto>>;
