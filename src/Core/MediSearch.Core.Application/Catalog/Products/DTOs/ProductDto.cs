namespace MediSearch.Core.Application.Catalog.Products.DTOs;

public record ProductDto
{
    public required Guid Id { get; init; }
    public required Guid ClassificationId { get; init; }
    public required IReadOnlyList<Guid> CategoryIds { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required double PriceAmount { get; init; }
    public required string PriceCurrency { get; init; }
    public required int Quantity { get; init; }
    public required IReadOnlyList<string> ImageKeys { get; init; }
}
