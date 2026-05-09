using FluentValidation;
using MediSearch.Core.Domain.Companies;

namespace MediSearch.Core.Application.Catalog.Products.Queries.GetProductPreviewsWithSeller;

public sealed class GetProductPreviewsWithSellerQueryValidator
    : AbstractValidator<GetProductPreviewsWithSellerQuery>
{
    private static readonly HashSet<int> CompanyTypeIds =
    [
        .. CompanyType.List().Select(type => type.Id),
    ];

    public GetProductPreviewsWithSellerQueryValidator()
    {
        RuleFor(v => v.CompanyType)
            .Must(type => CompanyTypeIds.Contains(type!.Value))
            .WithCustomErrorCode(ApplicationErrorCodes.InvalidCompanyType)
            .When(v => v.CompanyType != null);
    }
}
