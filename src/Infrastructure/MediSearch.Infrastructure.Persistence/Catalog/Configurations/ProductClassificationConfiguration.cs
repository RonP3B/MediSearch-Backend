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

        builder.ComplexProperty(
            pc => pc.Name,
            name =>
            {
                name.Property(p => p.Value)
                    .HasColumnName("name")
                    .HasMaxLength(150);

                name.Property(p => p.Normalized)
                    .HasColumnName("normalized_name")
                    .HasMaxLength(150);
            }
        );

        builder.HasKey(pc => pc.Id);

        builder
            .HasMany(pc => pc.Categories)
            .WithOne()
            .HasForeignKey(c => c.ClassificationId)
            .HasPrincipalKey(pc => pc.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
