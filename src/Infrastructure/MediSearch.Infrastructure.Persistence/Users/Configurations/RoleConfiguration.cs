using MediSearch.Core.Domain.SharedKernel.ValueObjects;
using MediSearch.Core.Domain.Users;
using MediSearch.Core.Domain.Users.AccessControl;

namespace MediSearch.Infrastructure.Persistence.Users.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");

        builder.HasKey(r => r.Id);

        builder.Property(role => role.Name).HasMaxLength(50);

        builder.HasIndex(r => r.Name).IsUnique();

        builder
            .HasMany<User>()
            .WithMany(u => u.Roles)
            .UsingEntity<Dictionary<string, object>>(
                "user_roles",
                j => j.HasOne<User>().WithMany().HasForeignKey("user_id"),
                j => j.HasOne<Role>().WithMany().HasForeignKey("role_id"),
                j =>
                {
                    j.HasKey("user_id", "role_id");
                    j.Property<int>("role_id").HasColumnName("role_id");
                    j.Property<EntityId<User>>("user_id").HasColumnName("user_id");
                    j.HasIndex("role_id", "user_id");
                }
            );

        builder.HasData(
            Role.SystemAdmin,
            Role.CompanyOwner,
            Role.CompanyManager,
            Role.CompanyMember,
            Role.Client
        );
    }
}
