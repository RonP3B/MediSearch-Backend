namespace MediSearch.Core.Application.Catalog.ProductClassifications.Constants;

internal static class ProductClassificationCacheKeys
{
    public const string AllProductClassifications = "product-classifications:all";

    public static string ClassificationCategories(Guid classificationId) =>
        $"product-classifications:{classificationId}:categories";
}
