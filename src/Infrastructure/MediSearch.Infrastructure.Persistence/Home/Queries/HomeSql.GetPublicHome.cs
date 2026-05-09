using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Infrastructure.Persistence.Home.Queries;

internal static partial class HomeSql
{
    public const string GetPublicHome =
        @$"
        -- 1) Latest Pharmacies
        SELECT
            c.id                          AS {nameof(CompanySummaryDto.Id)},
            c.name                        AS {nameof(CompanySummaryDto.Name)},
            c.image_key                   AS {nameof(CompanySummaryDto.ImageKey)},
            c.province                    AS {nameof(CompanySummaryDto.Province)},
            c.municipality                AS {nameof(CompanySummaryDto.Municipality)},
            c.address                     AS {nameof(CompanySummaryDto.Address)},
            NULL                          AS {nameof(CompanySummaryDto.IsFavoritedByCurrentUser)}
        FROM companies c
        WHERE c.company_type_id = @PharmacyCompanyTypeId
        ORDER BY c.created_at DESC
        LIMIT 10;
 
        -- 2) Latest Laboratories
        SELECT
            c.id                          AS {nameof(CompanySummaryDto.Id)},
            c.name                        AS {nameof(CompanySummaryDto.Name)},
            c.image_key                   AS {nameof(CompanySummaryDto.ImageKey)},
            c.province                    AS {nameof(CompanySummaryDto.Province)},
            c.municipality                AS {nameof(CompanySummaryDto.Municipality)},
            c.address                     AS {nameof(CompanySummaryDto.Address)},
            NULL                          AS {nameof(CompanySummaryDto.IsFavoritedByCurrentUser)}
        FROM companies c
        WHERE c.company_type_id = @LaboratoryCompanyTypeId
        ORDER BY c.created_at DESC
        LIMIT 10;
 
        -- 3) Latest Laboratory Products
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
            NULL                          AS {nameof(ProductPreviewDto.IsFavoritedByCurrentUser)}
        FROM products p
        JOIN companies c ON c.id = p.company_id
        WHERE c.company_type_id = @LaboratoryCompanyTypeId
        ORDER BY p.created_at DESC
        LIMIT 10;
 
        -- 4) Latest Pharmacy Products
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
            NULL                          AS {nameof(ProductPreviewDto.IsFavoritedByCurrentUser)}
        FROM products p
        JOIN companies c ON c.id = p.company_id
        WHERE c.company_type_id = @PharmacyCompanyTypeId
        ORDER BY p.created_at DESC
        LIMIT 10;
        ";
}
