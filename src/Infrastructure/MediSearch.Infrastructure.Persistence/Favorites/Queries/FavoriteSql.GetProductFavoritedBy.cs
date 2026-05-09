using MediSearch.Core.Application.Favorites.DTOs;

namespace MediSearch.Infrastructure.Persistence.Favorites.Queries;

internal static partial class FavoriteSql
{
    public const string GetProductFavoritedBy =
        $@"
        SELECT
            c.id AS {nameof(ProductFavoritedByDto.CompanyId)},
            c.name AS {nameof(ProductFavoritedByDto.CompanyName)},
            p.name AS {nameof(ProductFavoritedByDto.ProductName)},
            COALESCE(u.username, favoriter_company.name) AS {nameof(ProductFavoritedByDto.FavoriterName)}
        FROM products p
        JOIN companies c ON c.id = p.company_id
        -- Favoriters are polymorphic agents: either a client user or a company.
        LEFT JOIN users u
            ON u.id = @FavoriterAgentId
           AND @FavoriterAgentTypeId = @UserAgentTypeId
        LEFT JOIN companies favoriter_company
            ON favoriter_company.id = @FavoriterAgentId
           AND @FavoriterAgentTypeId = @CompanyAgentTypeId
        WHERE p.id = @ProductId
          AND COALESCE(u.id, favoriter_company.id) IS NOT NULL;
        ";
}
