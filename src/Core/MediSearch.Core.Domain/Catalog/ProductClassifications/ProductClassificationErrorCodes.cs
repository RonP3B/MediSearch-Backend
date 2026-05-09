namespace MediSearch.Core.Domain.Catalog.ProductClassifications;

internal static class ProductClassificationErrorCodes
{
    public const string NameTooLong = "Domain.Catalog.ProductClassifications.NameTooLong";
    public const string NameTooShort = "Domain.Catalog.ProductClassifications.NameTooShort";
    public const string CategoryAlreadyAdded =
        "Domain.Catalog.ProductClassifications.CategoryAlreadyAdded";
    public const string CategoryNotFound = "Domain.Catalog.ProductClassifications.CategoryNotFound";
    public const string CategoryNotInClassification =
        "Domain.Catalog.ProductClassifications.CategoryNotInClassification";
    public const string CategoryNameAlreadyExists =
        "Domain.Catalog.ProductClassifications.CategoryNameAlreadyExists";
}
