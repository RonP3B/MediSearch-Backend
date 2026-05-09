namespace MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;

public sealed record ClassificationCategoryDto
{
    public required Guid Id { get; init; }
    public required Guid ClassificationId { get; init; }
    public required string Name { get; init; }
}
