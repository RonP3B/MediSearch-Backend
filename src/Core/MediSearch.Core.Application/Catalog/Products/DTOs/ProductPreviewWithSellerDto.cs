namespace MediSearch.Core.Application.Catalog.Products.DTOs;

public sealed record ProductPreviewWithSellerDto : ProductPreviewDto
{
    public required string CompanyName { get; init; }
    public required string CompanyProvince { get; init; }
}
