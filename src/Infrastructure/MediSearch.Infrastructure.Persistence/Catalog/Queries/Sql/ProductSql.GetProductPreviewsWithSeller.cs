using MediSearch.Core.Application.Catalog.Products.DTOs;

namespace MediSearch.Infrastructure.Persistence.Catalog.Queries.Sql;

internal static partial class ProductSql
{
    public const string GetProductPreviewsWithSeller =
        @$"
         SELECT
            p.id                         AS {nameof(ProductPreviewWithSellerDto.Id)},
            p.classification_id          AS {nameof(ProductPreviewWithSellerDto.ClassificationId)},
            p.name                       AS {nameof(ProductPreviewWithSellerDto.Name)},
            p.price_amount               AS {nameof(ProductPreviewWithSellerDto.PriceAmount)},
            p.price_currency             AS {nameof(ProductPreviewWithSellerDto.PriceCurrency)},
            p.quantity                   AS {nameof(ProductPreviewWithSellerDto.Quantity)},
            c.name                       AS {nameof(ProductPreviewWithSellerDto.CompanyName)},
            c.province                   AS {nameof(ProductPreviewWithSellerDto.CompanyProvince)},
            (
                SELECT pi.image_key FROM product_images pi
                WHERE pi.product_id = p.id AND pi.ordinal = 1
            )                            AS {nameof(ProductPreviewWithSellerDto.ImageKey)},
            CASE
                WHEN @FavoriterId IS NULL THEN NULL
                ELSE EXISTS
                (
                    SELECT 1 FROM product_favorites pf
                      WHERE pf.product_id = p.id 
                        AND pf.favoriter_type_id = @FavoriterTypeId
                        AND pf.favoriter_id = @FavoriterId
                )
            END                          AS {nameof(ProductPreviewWithSellerDto.IsFavoritedByCurrentUser)},
            COALESCE(
                ARRAY_AGG(pcc.category_id ORDER BY pcc.category_id) 
                FILTER (WHERE pcc.category_id IS NOT NULL),
                ARRAY[]::uuid[]
            )                            AS {nameof(ProductPreviewWithSellerDto.CategoryIds)}
        FROM products p
        JOIN companies c ON c.id = p.company_id
        LEFT JOIN products_classification_categories pcc ON pcc.product_id = p.id
        WHERE (@CompanyType IS NULL OR c.company_type_id = @CompanyType)
        GROUP BY
            p.id,
            p.classification_id,
            p.name,
            p.price_amount,
            p.price_currency,
            p.quantity,
            c.name,
            c.province
        ORDER BY p.name;
        ";
}
