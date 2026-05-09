namespace MediSearch.Core.Domain.Users.AccessControl;

public static class PermissionCodes
{
    public const string ModifyCompany = "company:update";
    public const string AddCompanyUser = "company:add-user";
    public const string RemoveCompanyUser = "company:remove-user";
    public const string GetCompanyDashboard = "company:read-dashboard";
    public const string AddCompanyFavorite = "company-favorite:add";
    public const string RemoveCompanyFavorite = "company-favorite:remove";
    public const string AddProductFavorite = "product-favorite:add";
    public const string RemoveProductFavorite = "product-favorite:remove";
    public const string AddProduct = "product:add";
    public const string ModifyProduct = "product:update";
    public const string RemoveProduct = "product:remove";
    public const string RemoveUser = "user:remove";
    public const string RemoveCompany = "company:remove";
    public const string CreateClassificationCategory = "classification-category:create";
    public const string UpdateClassificationCategory = "classification-category:update";
    public const string DeleteClassificationCategory = "classification-category:delete";
    public const string CreateProductClassification = "product-classification:create";
    public const string UpdateProductClassification = "product-classification:update";
    public const string DeleteProductClassification = "product-classification:delete";
}
