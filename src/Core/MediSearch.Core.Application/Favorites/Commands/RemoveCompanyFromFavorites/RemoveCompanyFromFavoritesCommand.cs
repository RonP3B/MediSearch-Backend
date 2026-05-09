namespace MediSearch.Core.Application.Favorites.Commands.RemoveCompanyFromFavorites;

[Authorize(Permission = PermissionCodes.RemoveCompanyFavorite)]
public sealed record RemoveCompanyFromFavoritesCommand(Guid CompanyId) : ICommand;
