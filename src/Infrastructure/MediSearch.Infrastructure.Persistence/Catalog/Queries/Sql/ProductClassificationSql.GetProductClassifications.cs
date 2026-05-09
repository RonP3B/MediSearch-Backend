using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;

namespace MediSearch.Infrastructure.Persistence.Catalog.Queries.Sql;

internal static partial class ProductClassificationSql
{
    public const string GetProductClassifications =
        @$"
        SELECT
            pc.id                         AS {nameof(ProductClassificationDto.Id)},
            pc.name                       AS {nameof(ProductClassificationDto.Name)}
        FROM product_classifications pc
        ORDER BY pc.name
        ";
}
