namespace MediSearch.Core.Application.Favorites.DTOs;

public sealed record CompanyFavoritedByDto
{
    public required string CompanyName { get; init; }
    public required string FavoriterName { get; init; }
}
