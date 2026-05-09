using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Infrastructure.Persistence.Favorites.Queries;

internal static partial class FavoriteSql
{
    public const string GetAllFavorites =
        $@"
        -- 1) Product favorites
        SELECT
            p.id                         AS {nameof(ProductPreviewDto.Id)},
            p.classification_id          AS {nameof(ProductPreviewDto.ClassificationId)},
            p.name                       AS {nameof(ProductPreviewDto.Name)},
            p.price_amount               AS {nameof(ProductPreviewDto.PriceAmount)},
            p.price_currency             AS {nameof(ProductPreviewDto.PriceCurrency)},
            p.quantity                   AS {nameof(ProductPreviewDto.Quantity)},
            (
                SELECT pi.image_key FROM product_images pi
                WHERE pi.product_id = p.id AND pi.ordinal = 1
            )                            AS {nameof(ProductPreviewDto.ImageKey)},
            TRUE                         AS {nameof(ProductPreviewDto.IsFavoritedByCurrentUser)},
            COALESCE(
                ARRAY_AGG(pcc.category_id ORDER BY pcc.category_id) 
                FILTER (WHERE pcc.category_id IS NOT NULL),
                ARRAY[]::uuid[]
            )                            AS {nameof(ProductPreviewDto.CategoryIds)}        
        FROM product_favorites pf
        INNER JOIN products p ON p.id = pf.product_id
        LEFT JOIN products_classification_categories pcc ON pcc.product_id = p.id
        WHERE pf.favoriter_type_id = @FavoriterTypeId
          AND pf.favoriter_id = @FavoriterId
        GROUP BY
            p.id,
            p.classification_id,
            p.name,
            p.price_amount, 
            p.price_currency,
            p.quantity
        ORDER BY p.name;
 
        -- 2) Company favorites
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
