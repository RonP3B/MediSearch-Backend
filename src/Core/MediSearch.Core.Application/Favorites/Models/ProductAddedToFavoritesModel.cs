namespace MediSearch.Core.Application.Favorites.Models;

public sealed record ProductAddedToFavoritesModel(
    string CompanyName,
    string ProductName,
    string FavoriterName
);
