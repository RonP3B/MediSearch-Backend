using MediSearch.Core.Domain.Catalog.ProductClassifications;
using MediSearch.Infrastructure.Persistence.Catalog.ValueConvertors;

namespace MediSearch.Infrastructure.Persistence.Catalog.Configurations;

internal sealed class ProductClassificationConfiguration
    : IEntityTypeConfiguration<ProductClassification>
{
    public void Configure(EntityTypeBuilder<ProductClassification> builder)
    {
        builder.ToTable("product_classifications");

        builder
            .Property(pc => pc.Id)
            .HasConversion(new EntityIdValueConverter<ProductClassification>())
            .ValueGeneratedNever();

        builder
            .Property(pc => pc.Name)
            .HasConversion(new ClassificationNameValueConverter())
            .HasMaxLength(150);

        builder.HasKey(pc => pc.Id);
        builder.HasIndex(pc => pc.Name).IsUnique();

        builder
            .HasMany(pc => pc.Categories)
            .WithOne()
            .HasForeignKey(c => c.ClassificationId)
            .HasPrincipalKey(pc => pc.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
