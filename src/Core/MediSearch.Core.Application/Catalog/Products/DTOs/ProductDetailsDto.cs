using MediSearch.Core.Application.Catalog.Comments.DTOs;
using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;
using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Core.Application.Catalog.Products.DTOs;

public sealed record ProductDetailsDto : ProductDto
{
    public required ProductClassificationDto Classification { get; init; }
    public required IReadOnlyList<ClassificationCategoryDto> Categories { get; init; }
    public required CompanySummaryDto Company { get; init; }
    public required IReadOnlyList<CommentDto> Comments { get; init; }
}
