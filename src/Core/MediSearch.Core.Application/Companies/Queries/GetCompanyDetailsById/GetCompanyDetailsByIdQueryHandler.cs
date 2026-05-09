using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Core.Application.Companies.Ports;
using MediSearch.Core.Domain.Companies;

namespace MediSearch.Core.Application.Companies.Queries.GetCompanyDetailsById;

public sealed class GetCompanyDetailsByIdQueryHandler(
    ICompanyQueryService companyQueryService,
    ICurrentUser currentUser
) : IQueryHandler<GetCompanyDetailsByIdQuery, CompanyDetailsDto>
{
    private readonly ICompanyQueryService _companyQueryService = companyQueryService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<CompanyDetailsDto> Handle(
        GetCompanyDetailsByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        var currentAgent = _currentUser.ToAgentOrNull();

        var companyDetails = await _companyQueryService.GetCompanyDetailsByIdOrDefaultAsync(
            query.CompanyId,
            currentAgent?.AgentId,
            currentAgent?.AgentTypeId,
            cancellationToken
        );

        if (companyDetails is null)
        {
            throw NotFoundException.Entity(nameof(Company), nameof(Company.Id), query.CompanyId);
        }

        return companyDetails;
    }
}
