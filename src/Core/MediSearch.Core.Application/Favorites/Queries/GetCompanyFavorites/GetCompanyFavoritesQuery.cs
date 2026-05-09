using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Core.Application.Favorites.Queries.GetCompanyFavorites;

[Authorize]
public sealed record GetCompanyFavoritesQuery : IQuery<IReadOnlyList<CompanySummaryDto>>;
