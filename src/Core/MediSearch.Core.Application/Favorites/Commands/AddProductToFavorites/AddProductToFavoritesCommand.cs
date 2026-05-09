namespace MediSearch.Core.Application.Favorites.Commands.AddProductToFavorites;

[Authorize(Permission = PermissionCodes.AddProductFavorite)]
public sealed record AddProductToFavoritesCommand(Guid ProductId) : ICommand;
