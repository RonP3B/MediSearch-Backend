using MediSearch.Core.Application.Catalog.Products.DTOs;

namespace MediSearch.Infrastructure.Persistence.Favorites.Queries;

internal static partial class FavoriteSql
{
    public const string GetProductFavorites =
        $@"
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
        ";
}
