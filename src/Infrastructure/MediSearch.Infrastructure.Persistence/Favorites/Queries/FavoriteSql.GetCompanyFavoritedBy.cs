using MediSearch.Core.Application.Favorites.DTOs;

namespace MediSearch.Infrastructure.Persistence.Favorites.Queries;

internal static partial class FavoriteSql
{
    public const string GetCompanyFavoritedBy =
        $@"
        SELECT
            c.name AS {nameof(CompanyFavoritedByDto.CompanyName)},
            COALESCE(u.username, favoriter_company.name) AS {nameof(CompanyFavoritedByDto.FavoriterName)}
        FROM companies c
        -- Favoriters are polymorphic agents: either a client user or a company.
        LEFT JOIN users u
            ON u.id = @FavoriterAgentId
           AND @FavoriterAgentTypeId = @UserAgentTypeId
        LEFT JOIN companies favoriter_company
            ON favoriter_company.id = @FavoriterAgentId
           AND @FavoriterAgentTypeId = @CompanyAgentTypeId
        WHERE c.id = @CompanyId
          AND COALESCE(u.id, favoriter_company.id) IS NOT NULL;
        ";
}
