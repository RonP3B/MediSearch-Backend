using MediSearch.Core.Domain.Companies;
using MediSearch.Infrastructure.Persistence.Companies.ValueConverters;

namespace MediSearch.Infrastructure.Persistence.Companies.Configurations;

internal sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.ToTable("companies");

        builder
            .Property(c => c.Id)
            .HasConversion(new EntityIdValueConverter<Company>())
            .ValueGeneratedNever();

        builder
            .Property(c => c.Name)
            .HasConversion(new CompanyNameValueConverter())
            .HasMaxLength(100);

        builder
            .Property(c => c.CeoName)
            .HasConversion(new CompanyCeoNameValueConverter())
            .HasMaxLength(150);

        builder
            .Property(c => c.ImageKey)
            .HasConversion(new AssetKeyValueConverter())
            .HasMaxLength(500);

        builder.Property(c => c.Email).HasConversion(new EmailValueConverter()).HasMaxLength(320);

        builder
            .Property(c => c.PhoneNumber)
            .HasConversion(new PhoneNumberValueConverter())
            .HasMaxLength(14);

        builder
            .Property(c => c.Website)
            .HasConversion(new NullableUrlValueConverter())
            .HasMaxLength(500);

        builder
            .Property(c => c.Facebook)
            .HasConversion(new NullableUrlValueConverter())
            .HasMaxLength(500);

        builder
            .Property(c => c.Twitter)
            .HasConversion(new NullableUrlValueConverter())
            .HasMaxLength(500);

        builder
            .Property(c => c.Instagram)
            .HasConversion(new NullableUrlValueConverter())
            .HasMaxLength(500);

        builder.ComplexProperty(
            c => c.Location,
            loc =>
            {
                loc.Property(x => x.Province).HasColumnName("province").HasMaxLength(200);
                loc.Property(x => x.Municipality).HasColumnName("municipality").HasMaxLength(200);
                loc.Property(x => x.Address).HasColumnName("address").HasMaxLength(500);
            }
        );

        builder.HasKey(x => x.Id);
        builder.HasIndex(c => c.CompanyTypeId);
        builder.HasIndex(c => new { c.CompanyTypeId, c.CreatedAt });
        builder.HasIndex(c => c.Name).IsUnique();
        builder.HasIndex(c => c.Email).IsUnique();

        builder
            .HasOne<CompanyType>()
            .WithMany()
            .HasForeignKey(c => c.CompanyTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
