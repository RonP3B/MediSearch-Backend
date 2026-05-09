namespace MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;

public sealed record ProductClassificationDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
}
