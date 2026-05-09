using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Infrastructure.Persistence.Home.Queries;

internal static partial class HomeSql
{
    public const string GetClientHome =
        @$"
        -- 1) Latest 10 Products
        SELECT
            p.id                          AS {nameof(ProductPreviewDto.Id)},
            p.classification_id           AS {nameof(ProductPreviewDto.ClassificationId)},
            p.name                        AS {nameof(ProductPreviewDto.Name)},
            (
                SELECT pi.image_key FROM product_images pi
                WHERE pi.product_id = p.id AND pi.ordinal = 1
            )                             AS {nameof(ProductPreviewDto.ImageKey)},
            p.price_amount                AS {nameof(ProductPreviewDto.PriceAmount)},
            p.price_currency              AS {nameof(ProductPreviewDto.PriceCurrency)},
            p.quantity                    AS {nameof(ProductPreviewDto.Quantity)},
            EXISTS (
                SELECT 1 
                FROM product_favorites pf 
                WHERE pf.product_id = p.id 
                  AND pf.favoriter_type_id = @UserAgentTypeId
                  AND pf.favoriter_id = @UserId
            )                             AS {nameof(ProductPreviewDto.IsFavoritedByCurrentUser)}
        FROM products p
        JOIN companies c ON c.id = p.company_id
        WHERE c.company_type_id = @PharmacyCompanyTypeId
        ORDER BY p.created_at DESC
        LIMIT 10;
 
        -- 2) Pharmacies in user's province (10)
        SELECT
            c.id                          AS {nameof(CompanySummaryDto.Id)},
            c.name                        AS {nameof(CompanySummaryDto.Name)},
            c.image_key                   AS {nameof(CompanySummaryDto.ImageKey)},
            c.province                    AS {nameof(CompanySummaryDto.Province)},
            c.municipality                AS {nameof(CompanySummaryDto.Municipality)},
            c.address                     AS {nameof(CompanySummaryDto.Address)},
            EXISTS (
                SELECT 1 
                FROM company_favorites cf 
                WHERE cf.company_id = c.id 
                  AND cf.favoriter_type_id = @UserAgentTypeId
                  AND cf.favoriter_id = @UserId
            )                             AS {nameof(CompanySummaryDto.IsFavoritedByCurrentUser)}
        FROM companies c
        WHERE c.company_type_id = @PharmacyCompanyTypeId
          AND c.province = (
              SELECT u.province 
              FROM users u 
              WHERE u.id = @UserId
          )
        ORDER BY c.created_at DESC
        LIMIT 10;
 
        -- 3) User's Favorite Products (10)
        SELECT
            p.id                          AS {nameof(ProductPreviewDto.Id)},
            p.classification_id           AS {nameof(ProductPreviewDto.ClassificationId)},
            p.name                        AS {nameof(ProductPreviewDto.Name)},
            (
                SELECT pi.image_key FROM product_images pi
                WHERE pi.product_id = p.id AND pi.ordinal = 1
            )                             AS {nameof(ProductPreviewDto.ImageKey)},
            p.price_amount                AS {nameof(ProductPreviewDto.PriceAmount)},
            p.price_currency              AS {nameof(ProductPreviewDto.PriceCurrency)},
            p.quantity                    AS {nameof(ProductPreviewDto.Quantity)},
            TRUE                          AS {nameof(ProductPreviewDto.IsFavoritedByCurrentUser)}
        FROM products p
        JOIN product_favorites pf ON pf.product_id = p.id
        WHERE pf.favoriter_type_id = @UserAgentTypeId
          AND pf.favoriter_id = @UserId
        ORDER BY pf.created_at DESC
        LIMIT 10;
 
        -- 4) User's Favorite Companies (10)
        SELECT
            c.id                          AS {nameof(CompanySummaryDto.Id)},
            c.name                        AS {nameof(CompanySummaryDto.Name)},
            c.image_key                   AS {nameof(CompanySummaryDto.ImageKey)},
            c.province                    AS {nameof(CompanySummaryDto.Province)},
            c.municipality                AS {nameof(CompanySummaryDto.Municipality)},
            c.address                     AS {nameof(CompanySummaryDto.Address)},
            TRUE                          AS {nameof(CompanySummaryDto.IsFavoritedByCurrentUser)}
        FROM companies c
        JOIN company_favorites cf ON cf.company_id = c.id
        WHERE cf.favoriter_type_id = @UserAgentTypeId
          AND cf.favoriter_id = @UserId
        ORDER BY cf.created_at DESC
        LIMIT 10;
        ";
}
