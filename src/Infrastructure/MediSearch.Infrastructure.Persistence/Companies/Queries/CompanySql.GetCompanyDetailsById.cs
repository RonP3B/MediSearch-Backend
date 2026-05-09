using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Infrastructure.Persistence.Companies.Queries;

internal static partial class CompanySql
{
    public const string GetCompanyDetailsById =
        @$"
        -- 1) Company basic info
        SELECT
            c.id                          AS {nameof(CompanyDto.Id)},
            c.name                        AS {nameof(CompanyDto.Name)},
            c.ceo_name                    AS {nameof(CompanyDto.CeoName)},
            c.province                    AS {nameof(CompanyDto.Province)},
            c.municipality                AS {nameof(CompanyDto.Municipality)},
            c.address                     AS {nameof(CompanyDto.Address)},
            c.image_key                   AS {nameof(CompanyDto.ImageKey)},
            c.email                       AS {nameof(CompanyDto.Email)},
            c.phone_number                AS {nameof(CompanyDto.PhoneNumber)},
            c.company_type_id             AS {nameof(CompanyDto.CompanyTypeId)},
            c.website                     AS {nameof(CompanyDto.Website)},
            c.facebook                    AS {nameof(CompanyDto.Facebook)},
            c.instagram                   AS {nameof(CompanyDto.Instagram)},
            c.twitter                     AS {nameof(CompanyDto.Twitter)}
        FROM companies c
        WHERE c.id = @CompanyId;
 
        -- 2) Products previews
        SELECT
            p.id                          AS {nameof(ProductPreviewDto.Id)},
            p.classification_id           AS {nameof(ProductPreviewDto.ClassificationId)},
            p.name                        AS {nameof(ProductPreviewDto.Name)},
            p.price_amount                AS {nameof(ProductPreviewDto.PriceAmount)},
            p.price_currency              AS {nameof(ProductPreviewDto.PriceCurrency)},
            p.quantity                    AS {nameof(ProductPreviewDto.Quantity)},
            (
                SELECT pi.image_key FROM product_images pi
                WHERE pi.product_id = p.id AND pi.ordinal = 1
            )                             AS {nameof(ProductPreviewDto.ImageKey)},
            CASE
                WHEN @FavoriterId IS NULL THEN NULL
                ELSE EXISTS
                (
                    SELECT 1 FROM product_favorites pf
                      WHERE pf.product_id = p.id 
                        AND pf.favoriter_type_id = @FavoriterTypeId
                        AND pf.favoriter_id = @FavoriterId
                )
            END                           AS {nameof(ProductPreviewDto.IsFavoritedByCurrentUser)},
            COALESCE(
                ARRAY_AGG(pcc.category_id ORDER BY pcc.category_id) 
                FILTER (WHERE pcc.category_id IS NOT NULL),
                ARRAY[]::uuid[]
            )                             AS {nameof(ProductPreviewDto.CategoryIds)}
        FROM products p
        LEFT JOIN products_classification_categories pcc ON pcc.product_id = p.id
        WHERE p.company_id = @CompanyId
        GROUP BY
            p.id,
            p.classification_id,
            p.name,
            p.price_amount, 
            p.price_currency,
            p.quantity
        ORDER BY p.name;
        ";
}
