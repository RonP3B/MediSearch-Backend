using MediSearch.Core.Application.Catalog.Products.DTOs;

namespace MediSearch.Core.Application.Catalog.Products.Queries.GetProductPreviewsByCompanyId;

public sealed record GetProductPreviewsByCompanyIdQuery(Guid CompanyId)
    : IQuery<IReadOnlyList<ProductPreviewDto>>;
