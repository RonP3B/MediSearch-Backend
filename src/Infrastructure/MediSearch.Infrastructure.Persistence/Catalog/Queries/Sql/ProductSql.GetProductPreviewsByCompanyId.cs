using MediSearch.Core.Application.Catalog.Products.DTOs;

namespace MediSearch.Infrastructure.Persistence.Catalog.Queries.Sql;

internal static partial class ProductSql
{
    public const string GetProductPreviewsByCompanyId =
        @$"
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
            CASE
                WHEN @FavoriterId IS NULL THEN NULL
                ELSE EXISTS
                (
                    SELECT 1 FROM product_favorites pf
                      WHERE pf.product_id = p.id 
                        AND pf.favoriter_type_id = @FavoriterTypeId
                        AND pf.favoriter_id = @FavoriterId
                )
            END                          AS {nameof(ProductPreviewDto.IsFavoritedByCurrentUser)},
            COALESCE(
                ARRAY_AGG(pcc.category_id ORDER BY pcc.category_id) 
                FILTER (WHERE pcc.category_id IS NOT NULL),
                ARRAY[]::uuid[]
            )                            AS {nameof(ProductPreviewDto.CategoryIds)}
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
