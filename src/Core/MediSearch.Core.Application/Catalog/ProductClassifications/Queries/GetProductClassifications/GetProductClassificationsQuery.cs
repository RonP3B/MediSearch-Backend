using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Queries.GetProductClassifications;

public sealed record GetProductClassificationsQuery
    : IQuery<IReadOnlyList<ProductClassificationDto>>;
