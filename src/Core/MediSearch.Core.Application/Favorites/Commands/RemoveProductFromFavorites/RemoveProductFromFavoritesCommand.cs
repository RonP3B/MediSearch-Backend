namespace MediSearch.Core.Application.Favorites.Commands.RemoveProductFromFavorites;

[Authorize(Permission = PermissionCodes.RemoveProductFavorite)]
public sealed record RemoveProductFromFavoritesCommand(Guid ProductId) : ICommand;
