using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Core.Application.Home.DTOs;

public sealed record ClientHomeDto
{
    public required IReadOnlyList<ProductPreviewDto> LastestProducts { get; init; }
    public required IReadOnlyList<CompanySummaryDto> PharmaciesInYourProvince { get; init; }
    public required IReadOnlyList<ProductPreviewDto> LatestFavoritedProducts { get; init; }
    public required IReadOnlyList<CompanySummaryDto> LatestFavoritedCompanies { get; init; }
}
