using MediSearch.Core.Domain.Users.AccessControl;

namespace MediSearch.Infrastructure.Persistence.Users.Configurations;

internal sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(permission => permission.Code);

        builder.Property(permission => permission.Code).HasMaxLength(100);

        builder.HasData(
            Permission.ModifyCompany,
            Permission.AddCompanyUser,
            Permission.RemoveCompanyUser,
            Permission.GetCompanyDashboard,
            Permission.AddCompanyFavorite,
            Permission.RemoveCompanyFavorite,
            Permission.AddProductFavorite,
            Permission.RemoveProductFavorite,
            Permission.AddProduct,
            Permission.ModifyProduct,
            Permission.RemoveProduct,
            Permission.RemoveUser,
            Permission.RemoveCompany,
            Permission.CreateProductCategory,
            Permission.UpdateProductCategory,
            Permission.DeleteProductCategory,
            Permission.CreateProductClassification,
            Permission.UpdateProductClassification,
            Permission.DeleteProductClassification
        );

        builder
            .HasMany<Role>()
            .WithMany()
            .UsingEntity(joinBuilder =>
            {
                joinBuilder.ToTable("role_permissions");

                joinBuilder.HasIndex(["PermissionCode", "RoleId"]);

                joinBuilder.HasData(
                    // System Admin Permissions
                    CreateRolePermission(Role.SystemAdmin, Permission.CreateProductCategory),
                    CreateRolePermission(Role.SystemAdmin, Permission.UpdateProductCategory),
                    CreateRolePermission(Role.SystemAdmin, Permission.DeleteProductCategory),
                    CreateRolePermission(Role.SystemAdmin, Permission.CreateProductClassification),
                    CreateRolePermission(Role.SystemAdmin, Permission.UpdateProductClassification),
                    CreateRolePermission(Role.SystemAdmin, Permission.DeleteProductClassification),
                    CreateRolePermission(Role.SystemAdmin, Permission.RemoveUser),
                    CreateRolePermission(Role.SystemAdmin, Permission.RemoveCompany),
                    // Company Owner Roles Permissions
                    CreateRolePermission(Role.CompanyOwner, Permission.ModifyCompany),
                    CreateRolePermission(Role.CompanyOwner, Permission.AddCompanyUser),
                    CreateRolePermission(Role.CompanyOwner, Permission.RemoveCompanyUser),
                    CreateRolePermission(Role.CompanyOwner, Permission.GetCompanyDashboard),
                    CreateRolePermission(Role.CompanyOwner, Permission.AddCompanyFavorite),
                    CreateRolePermission(Role.CompanyOwner, Permission.RemoveCompanyFavorite),
                    CreateRolePermission(Role.CompanyOwner, Permission.AddProductFavorite),
                    CreateRolePermission(Role.CompanyOwner, Permission.RemoveProductFavorite),
                    CreateRolePermission(Role.CompanyOwner, Permission.AddProduct),
                    CreateRolePermission(Role.CompanyOwner, Permission.ModifyProduct),
                    CreateRolePermission(Role.CompanyOwner, Permission.RemoveProduct),
                    CreateRolePermission(Role.CompanyOwner, Permission.RemoveCompany),
                    // Company Manager Roles Permissions
                    CreateRolePermission(Role.CompanyManager, Permission.AddCompanyUser),
                    CreateRolePermission(Role.CompanyManager, Permission.RemoveCompanyUser),
                    CreateRolePermission(Role.CompanyManager, Permission.GetCompanyDashboard),
                    CreateRolePermission(Role.CompanyManager, Permission.AddCompanyFavorite),
                    CreateRolePermission(Role.CompanyManager, Permission.RemoveCompanyFavorite),
                    CreateRolePermission(Role.CompanyManager, Permission.AddProductFavorite),
                    CreateRolePermission(Role.CompanyManager, Permission.RemoveProductFavorite),
                    CreateRolePermission(Role.CompanyManager, Permission.AddProduct),
                    CreateRolePermission(Role.CompanyManager, Permission.ModifyProduct),
                    CreateRolePermission(Role.CompanyManager, Permission.RemoveProduct),
                    // Company Member Roles Permissions
                    CreateRolePermission(Role.CompanyMember, Permission.GetCompanyDashboard),
                    CreateRolePermission(Role.CompanyMember, Permission.AddProduct),
                    CreateRolePermission(Role.CompanyMember, Permission.ModifyProduct),
                    CreateRolePermission(Role.CompanyMember, Permission.RemoveProduct),
                    // Client Roles Permissions
                    CreateRolePermission(Role.Client, Permission.AddCompanyFavorite),
                    CreateRolePermission(Role.Client, Permission.RemoveCompanyFavorite),
                    CreateRolePermission(Role.Client, Permission.AddProductFavorite),
                    CreateRolePermission(Role.Client, Permission.RemoveProductFavorite)
                );
            });
    }

    private static object CreateRolePermission(Role role, Permission permission)
    {
        return new { RoleId = role.Id, PermissionCode = permission.Code };
    }
}
