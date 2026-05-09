namespace MediSearch.Core.Application.Favorites.DTOs;

public sealed record ProductFavoritedByDto
{
    public required Guid CompanyId { get; init; }
    public required string CompanyName { get; init; }
    public required string ProductName { get; init; }
    public required string FavoriterName { get; init; }
}
