using MediSearch.Core.Application.Favorites.DTOs;

namespace MediSearch.Core.Application.Favorites.Queries.GetAllFavorites;

[Authorize]
public sealed record GetAllFavoritesQuery : IQuery<FavoritesDto>;
