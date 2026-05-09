namespace MediSearch.Core.Application.Catalog.Products.DTOs;

public record ProductPreviewDto
{
    public required Guid Id { get; init; }
    public required Guid ClassificationId { get; init; }
    public required IReadOnlyList<Guid> CategoryIds { get; init; }
    public required string Name { get; init; }
    public required string ImageKey { get; init; }
    public required double PriceAmount { get; init; }
    public required string PriceCurrency { get; init; }
    public required int Quantity { get; init; }
    public bool? IsFavoritedByCurrentUser { get; init; } = null;
}
