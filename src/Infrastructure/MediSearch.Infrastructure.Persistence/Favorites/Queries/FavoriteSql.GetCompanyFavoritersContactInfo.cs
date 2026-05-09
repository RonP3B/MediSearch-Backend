using MediSearch.Core.Application.Users.DTOs;

namespace MediSearch.Infrastructure.Persistence.Favorites.Queries;

internal static partial class FavoriteSql
{
    public const string GetCompanyFavoritersContactInfo =
        $@"
        SELECT DISTINCT
            favoriters.email    AS {nameof(UserContactInfoDto.Email)},
            favoriters.full_name AS {nameof(UserContactInfoDto.FullName)},
            favoriters.username  AS {nameof(UserContactInfoDto.Username)}
        FROM
        (
            SELECT
                u.email,
                CONCAT(u.first_name, ' ', u.last_name) AS full_name,
                u.username
            FROM company_favorites cf
            JOIN users u
                ON cf.favoriter_type_id = @UserAgentTypeId
               AND u.id = cf.favoriter_id
            WHERE cf.company_id = @CompanyId

            UNION

            SELECT 
                cu.email,
                CONCAT(cu.first_name, ' ', cu.last_name) AS full_name,
                cu.username
            FROM company_favorites cf
            JOIN users cu
                ON cf.favoriter_type_id = @CompanyAgentTypeId
               AND cu.company_id = cf.favoriter_id
            WHERE cf.company_id = @CompanyId
        ) favoriters
        WHERE favoriters.email IS NOT NULL
          AND favoriters.email <> ''
        ORDER BY favoriters.email;
        ";
}
