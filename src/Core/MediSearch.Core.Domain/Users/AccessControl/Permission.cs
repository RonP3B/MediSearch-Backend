namespace MediSearch.Core.Domain.Users.AccessControl;

public sealed class Permission
{
    private Permission(string code)
    {
        Code = code;
    }

    public string Code { get; private set; }

    public static readonly Permission ModifyCompany = new(PermissionCodes.ModifyCompany);
    public static readonly Permission AddCompanyUser = new(PermissionCodes.AddCompanyUser);
    public static readonly Permission RemoveCompanyUser = new(PermissionCodes.RemoveCompanyUser);
    public static readonly Permission GetCompanyDashboard = new(
        PermissionCodes.GetCompanyDashboard
    );
    public static readonly Permission AddCompanyFavorite = new(PermissionCodes.AddCompanyFavorite);
    public static readonly Permission RemoveCompanyFavorite = new(
        PermissionCodes.RemoveCompanyFavorite
    );
    public static readonly Permission AddProductFavorite = new(PermissionCodes.AddProductFavorite);
    public static readonly Permission RemoveProductFavorite = new(
        PermissionCodes.RemoveProductFavorite
    );
    public static readonly Permission AddProduct = new(PermissionCodes.AddProduct);
    public static readonly Permission ModifyProduct = new(PermissionCodes.ModifyProduct);
    public static readonly Permission RemoveProduct = new(PermissionCodes.RemoveProduct);
    public static readonly Permission RemoveUser = new(PermissionCodes.RemoveUser);
    public static readonly Permission RemoveCompany = new(PermissionCodes.RemoveCompany);
    public static readonly Permission CreateProductCategory = new(
        PermissionCodes.CreateClassificationCategory
    );
    public static readonly Permission UpdateProductCategory = new(
        PermissionCodes.UpdateClassificationCategory
    );
    public static readonly Permission DeleteProductCategory = new(
        PermissionCodes.DeleteClassificationCategory
    );
    public static readonly Permission CreateProductClassification = new(
        PermissionCodes.CreateProductClassification
    );
    public static readonly Permission UpdateProductClassification = new(
        PermissionCodes.UpdateProductClassification
    );
    public static readonly Permission DeleteProductClassification = new(
        PermissionCodes.DeleteProductClassification
    );
}
