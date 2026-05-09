using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Infrastructure.Persistence.Favorites.Queries;

internal static partial class FavoriteSql
{
    public const string GetProductFavoritersContactInfo =
        $@"
        SELECT DISTINCT
            favoriters.email      AS {nameof(UserContactInfoDto.Email)},
            favoriters.full_name  AS {nameof(UserContactInfoDto.FullName)},
            favoriters.username   AS {nameof(UserContactInfoDto.Username)}
        FROM
        (
            SELECT 
                u.email,
                CONCAT(u.first_name, ' ', u.last_name) AS full_name,
                u.username
            FROM product_favorites pf
            JOIN users u
                ON pf.favoriter_type_id = @UserAgentTypeId
               AND u.id = pf.favoriter_id
            WHERE pf.product_id = @ProductId

            UNION

            SELECT 
                cu.email,
                CONCAT(cu.first_name, ' ', cu.last_name) AS full_name,
                cu.username
            FROM product_favorites pf
            JOIN users cu
                ON pf.favoriter_type_id = @CompanyAgentTypeId
               AND cu.company_id = pf.favoriter_id
            WHERE pf.product_id = @ProductId
        ) favoriters
        WHERE favoriters.email IS NOT NULL
          AND favoriters.email <> ''
        ORDER BY favoriters.email;
        ";
}
