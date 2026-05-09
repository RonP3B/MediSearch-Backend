namespace MediSearch.Core.Application.Favorites.Commands.AddCompanyToFavorites;

[Authorize(Permission = PermissionCodes.AddCompanyFavorite)]
public sealed record AddCompanyToFavoritesCommand(Guid CompanyId) : ICommand;
