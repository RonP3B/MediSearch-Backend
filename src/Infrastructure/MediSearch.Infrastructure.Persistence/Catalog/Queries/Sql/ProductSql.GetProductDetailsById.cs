using MediSearch.Core.Application.Catalog.Comments.DTOs;
using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;
using MediSearch.Core.Application.Catalog.Products.DTOs;
using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Infrastructure.Persistence.Catalog.Queries.Sql;

internal static partial class ProductSql
{
    public const string GetProductDetailsById =
        @$"
        -- 1) Product base
        SELECT
            p.id                         AS {nameof(ProductDto.Id)},
            p.classification_id          AS {nameof(ProductDto.ClassificationId)},
            p.name                       AS {nameof(ProductDto.Name)},
            p.description                AS {nameof(ProductDto.Description)},
            p.price_amount               AS {nameof(ProductDto.PriceAmount)},
            p.price_currency             AS {nameof(ProductDto.PriceCurrency)},
            p.quantity                   AS {nameof(ProductDto.Quantity)},
            COALESCE(
                ARRAY_AGG(pi.image_key ORDER BY pi.ordinal)
                FILTER (WHERE pi.image_key IS NOT NULL),
                ARRAY[]::text[]
            )                            AS {nameof(ProductDto.ImageKeys)}
        FROM products p
        LEFT JOIN product_images pi ON pi.product_id = p.id
        WHERE p.id = @ProductId
        GROUP BY
            p.id,
            p.classification_id,
            p.name,
            p.description,
            p.price_amount,
            p.price_currency,
            p.quantity;
 
        -- 2) Product classification
        SELECT
            pc.id                        AS {nameof(ProductClassificationDto.Id)},
            pc.name                      AS {nameof(ProductClassificationDto.Name)}
        FROM product_classifications pc
        WHERE pc.id = (SELECT classification_id FROM products WHERE id = @ProductId);
 
        -- 3) Categories assigned to this product
        SELECT
            cc.id                        AS {nameof(ClassificationCategoryDto.Id)},
            cc.classification_id         AS {nameof(ClassificationCategoryDto.ClassificationId)},
            cc.name                      AS {nameof(ClassificationCategoryDto.Name)}
        FROM classification_categories cc
        JOIN products_classification_categories pcc ON pcc.category_id = cc.id
        WHERE pcc.product_id = @ProductId
        ORDER BY cc.name;
 
        -- 4) Company summary
        SELECT
            c.id                         AS {nameof(CompanySummaryDto.Id)},
            c.name                       AS {nameof(CompanySummaryDto.Name)},
            c.image_key                  AS {nameof(CompanySummaryDto.ImageKey)},
            c.province                   AS {nameof(CompanySummaryDto.Province)},
            c.municipality               AS {nameof(CompanySummaryDto.Municipality)},
            c.address                    AS {nameof(CompanySummaryDto.Address)}
        FROM companies c
        WHERE c.id = (SELECT company_id FROM products WHERE id = @ProductId);
 
        -- 5) Comments for the product
        SELECT
            comm.id                      AS {nameof(CommentDto.Id)},
            comm.product_id              AS {nameof(CommentDto.ProductId)},
            comm.content                 AS {nameof(CommentDto.Content)},
            u.id                         AS {nameof(AuthorDto.Id)},
            u.username                   AS {nameof(AuthorDto.Username)},
            u.profile_image_key          AS {nameof(AuthorDto.ProfileImageKey)},
            comp.id                      AS {nameof(AuthorCompanyDto.Id)},
            comp.name                    AS {nameof(AuthorCompanyDto.Name)}
        FROM comments comm
        JOIN users u ON u.id = comm.user_id
        LEFT JOIN companies comp ON comp.id = u.company_id
        WHERE comm.product_id = @ProductId
        ORDER BY comm.created_at;
        ";
}
