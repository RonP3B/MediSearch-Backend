namespace MediSearch.Core.Domain.Catalog.Products;

internal static class ProductErrorCodes
{
    public const string ImageKeyRequired = "Domain.Catalog.Products.ImageKeyRequired";
    public const string QuantityBelowZero = "Domain.Catalog.Products.QuantityBelowZero";
    public const string PriceBelowMinimum = "Domain.Catalog.Products.PriceBelowMinimum";
    public const string UnsupportedCurrency = "Domain.Catalog.Products.UnsupportedCurrency";
    public const string CategoryIdRequired = "Domain.Catalog.Products.CategoryIdRequired";
    public const string ImageKeyLimitExceeded = "Domain.Catalog.Products.ImageKeyLimitExceeded";
    public const string ProductNameTooLong = "Domain.Catalog.Products.ProductNameTooLong";
    public const string ProductNameTooShort = "Domain.Catalog.Products.ProductNameTooShort";
    public const string ProductNameInvalid = "Domain.Catalog.Products.ProductNameInvalid";
}
