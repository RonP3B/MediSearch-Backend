using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;

namespace MediSearch.Core.Application.Catalog.ProductClassifications.Queries.GetProductClassificationCategories;

public sealed record GetProductClassificationCategoriesQuery(Guid ClassificationId)
    : IQuery<IReadOnlyList<ClassificationCategoryDto>>;
