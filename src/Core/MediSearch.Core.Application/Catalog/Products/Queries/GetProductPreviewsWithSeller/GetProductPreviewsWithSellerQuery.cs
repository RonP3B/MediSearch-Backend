using MediSearch.Core.Application.Catalog.Products.DTOs;

namespace MediSearch.Core.Application.Catalog.Products.Queries.GetProductPreviewsWithSeller;

public sealed record GetProductPreviewsWithSellerQuery(int? CompanyType)
    : IQuery<IReadOnlyList<ProductPreviewWithSellerDto>>;
