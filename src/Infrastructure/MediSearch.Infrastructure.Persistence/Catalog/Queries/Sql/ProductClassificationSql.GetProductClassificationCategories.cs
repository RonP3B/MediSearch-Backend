using MediSearch.Core.Application.Catalog.ProductClassifications.DTOs;

namespace MediSearch.Infrastructure.Persistence.Catalog.Queries.Sql;

internal static partial class ProductClassificationSql
{
    public const string GetProductClassificationCategories =
        @$"
        SELECT
            cc.id                         AS {nameof(ClassificationCategoryDto.Id)},
            cc.classification_id          AS {nameof(ClassificationCategoryDto.ClassificationId)},
            cc.name                       AS {nameof(ClassificationCategoryDto.Name)}
        FROM classification_categories cc
        WHERE cc.classification_id = @ClassificationId
        ORDER BY cc.name
        ";
}
