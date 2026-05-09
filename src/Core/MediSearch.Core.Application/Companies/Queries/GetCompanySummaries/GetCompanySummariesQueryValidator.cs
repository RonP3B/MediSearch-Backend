using FluentValidation;
using MediSearch.Core.Domain.Companies;

namespace MediSearch.Core.Application.Companies.Queries.GetCompanySummaries;

public sealed class GetCompanySummariesQueryValidator : AbstractValidator<GetCompanySummariesQuery>
{
    private static readonly HashSet<int> CompanyTypeIds =
    [
        .. CompanyType.List().Select(type => type.Id),
    ];

    public GetCompanySummariesQueryValidator()
    {
        RuleFor(v => v.CompanyType)
            .Must(type => CompanyTypeIds.Contains(type!.Value))
            .WithCustomErrorCode(ApplicationErrorCodes.InvalidCompanyType)
            .When(v => v.CompanyType != null);
    }
}
