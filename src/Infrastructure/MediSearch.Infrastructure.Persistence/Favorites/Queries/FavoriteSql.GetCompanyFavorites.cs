using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Infrastructure.Persistence.Favorites.Queries;

internal static partial class FavoriteSql
{
    public const string GetCompanyFavorites =
        $@"
        SELECT
            c.id                          AS {nameof(CompanySummaryDto.Id)},
            c.name                        AS {nameof(CompanySummaryDto.Name)},
            c.province                    AS {nameof(CompanySummaryDto.Province)},
            c.municipality                AS {nameof(CompanySummaryDto.Municipality)},
            c.address                     AS {nameof(CompanySummaryDto.Address)},
            c.image_key                   AS {nameof(CompanySummaryDto.ImageKey)},
            TRUE                          AS {nameof(CompanySummaryDto.IsFavoritedByCurrentUser)}
        FROM company_favorites cf
        INNER JOIN companies c ON c.id = cf.company_id
        WHERE cf.favoriter_type_id = @FavoriterTypeId 
            AND cf.favoriter_id = @FavoriterId
        ORDER BY c.name;
        ";
}
