using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Core.Application.Companies.Ports;

namespace MediSearch.Core.Application.Companies.Queries.GetCompanySummaries;

public sealed class GetCompanySummariesQueryHandler(
    ICompanyQueryService companyQueryService,
    ICurrentUser currentUser
) : IQueryHandler<GetCompanySummariesQuery, IReadOnlyList<CompanySummaryDto>>
{
    private readonly ICompanyQueryService _companyQueryService = companyQueryService;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<IReadOnlyList<CompanySummaryDto>> Handle(
        GetCompanySummariesQuery query,
        CancellationToken cancellationToken
    )
    {
        var currentAgent = _currentUser.ToAgentOrNull();

        return await _companyQueryService.GetCompanySummariesAsync(
            query.CompanyType,
            currentAgent?.AgentId,
            currentAgent?.AgentTypeId,
            cancellationToken
        );
    }
}
