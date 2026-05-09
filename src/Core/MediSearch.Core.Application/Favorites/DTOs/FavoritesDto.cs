using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Core.Application.Favorites.DTOs;

public sealed record FavoritesDto
{
    public required IReadOnlyList<ProductPreviewDto> FavoriteProducts { get; init; }
    public required IReadOnlyList<CompanySummaryDto> FavoriteCompanies { get; init; }
}
