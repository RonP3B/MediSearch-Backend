using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.Users;
using MediSearch.Infrastructure.Persistence.Users.ValueConverters;

namespace MediSearch.Infrastructure.Persistence.Users.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder
            .Property(u => u.Id)
            .HasConversion(new EntityIdValueConverter<User>())
            .ValueGeneratedNever();

        builder
            .Property(u => u.ExternalId)
            .HasConversion(new ExternalIdValueConverter())
            .HasMaxLength(200);

        builder.Property(u => u.Email).HasConversion(new EmailValueConverter()).HasMaxLength(320);

        builder
            .Property(u => u.PhoneNumber)
            .HasConversion(new PhoneNumberValueConverter())
            .HasMaxLength(14);

        builder
            .Property(u => u.ProfileImageKey)
            .HasConversion(new NullableAssetKeyValueConverter())
            .HasMaxLength(500);

        builder
            .Property(u => u.CompanyId)
            .HasConversion(new NullableEntityIdValueConverter<Company>());

        builder.ComplexProperty(
            u => u.FullName,
            fullName =>
            {
                fullName.Property(f => f.FirstName).HasColumnName("first_name").HasMaxLength(100);
                fullName.Property(f => f.LastName).HasColumnName("last_name").HasMaxLength(100);
            }
        );

        builder.ComplexProperty(
            u => u.Location,
            loc =>
            {
                loc.Property(x => x.Province).HasColumnName("province").HasMaxLength(200);
                loc.Property(x => x.Municipality).HasColumnName("municipality").HasMaxLength(200);
                loc.Property(x => x.Address).HasColumnName("address").HasMaxLength(500);
            }
        );

        builder.ComplexProperty(
            u => u.Username,
            username =>
            {
                username.Property(u => u.Value).HasColumnName("username").HasMaxLength(15);

                username
                    .Property(u => u.Normalized)
                    .HasColumnName("normalized_username")
                    .HasMaxLength(15);
            }
        );

        builder.HasKey(u => u.Id);

        builder.HasIndex(u => u.CompanyId).HasFilter("company_id IS NOT NULL");
        builder.HasIndex(u => new { u.CompanyId, u.Email }).HasFilter("company_id IS NOT NULL");
        builder.HasIndex(u => u.ExternalId).IsUnique();
        builder.HasIndex(u => u.Email).IsUnique();

        builder
            .HasOne<Company>()
            .WithMany()
            .HasForeignKey(u => u.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
