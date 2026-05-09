using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Core.Application.Home.DTOs;

public sealed record PublicHomeDto
{
    public required IReadOnlyList<CompanySummaryDto> LastestPharmacies { get; init; }
    public required IReadOnlyList<CompanySummaryDto> LastestLaboratories { get; init; }
    public required IReadOnlyList<ProductPreviewDto> LastestLaboratoryProducts { get; init; }
    public required IReadOnlyList<ProductPreviewDto> LastestPharmacyProducts { get; init; }
}
