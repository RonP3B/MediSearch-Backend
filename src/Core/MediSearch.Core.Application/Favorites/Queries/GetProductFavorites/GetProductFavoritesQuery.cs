using MediSearch.Core.Application.Catalog.Products.DTOs;

namespace MediSearch.Core.Application.Favorites.Queries.GetProductFavorites;

[Authorize]
public sealed record GetProductFavoritesQuery : IQuery<IReadOnlyList<ProductPreviewDto>>;
