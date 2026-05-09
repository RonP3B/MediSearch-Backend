using MediSearch.Core.Application.Catalog.Products.DTOs;

namespace MediSearch.Core.Application.Companies.DTOs;

public sealed record CompanyDetailsDto : CompanyDto
{
    public required IReadOnlyList<ProductPreviewDto> Products { get; init; }
}
