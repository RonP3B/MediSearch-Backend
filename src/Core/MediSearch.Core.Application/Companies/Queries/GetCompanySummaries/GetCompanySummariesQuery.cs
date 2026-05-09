using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Core.Application.Companies.Queries.GetCompanySummaries;

public sealed record GetCompanySummariesQuery(int? CompanyType)
    : IQuery<IReadOnlyList<CompanySummaryDto>>;
