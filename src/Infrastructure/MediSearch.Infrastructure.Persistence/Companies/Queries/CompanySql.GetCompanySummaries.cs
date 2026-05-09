using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Infrastructure.Persistence.Companies.Queries;

internal static partial class CompanySql
{
    public const string GetCompanySummaries =
        @$"
        SELECT
            c.id                          AS {nameof(CompanySummaryDto.Id)},
            c.name                        AS {nameof(CompanySummaryDto.Name)},
            c.province                    AS {nameof(CompanySummaryDto.Province)},
            c.municipality                AS {nameof(CompanySummaryDto.Municipality)},
            c.address                     AS {nameof(CompanySummaryDto.Address)},
            c.image_key                   AS {nameof(CompanySummaryDto.ImageKey)},
            CASE
                WHEN @FavoriterId IS NULL THEN NULL
                ELSE EXISTS
                  (
                    SELECT 1 FROM company_favorites cf
                    WHERE cf.company_id = c.id
                        AND cf.favoriter_type_id = @FavoriterTypeId
                        AND cf.favoriter_id = @FavoriterId
                  )
            END                           AS {nameof(CompanySummaryDto.IsFavoritedByCurrentUser)}
        FROM companies c
        WHERE (@CompanyType IS NULL OR c.company_type_id = @CompanyType)
        ";
}
