using MediSearch.Core.Domain.Catalog.ProductClassifications;
using MediSearch.Infrastructure.Persistence.Catalog.ValueConvertors;

namespace MediSearch.Infrastructure.Persistence.Catalog.Configurations;

internal sealed class ClassificationCategoryConfiguration
    : IEntityTypeConfiguration<ClassificationCategory>
{
    public void Configure(EntityTypeBuilder<ClassificationCategory> builder)
    {
        builder.ToTable("classification_categories");

        builder
            .Property(c => c.Id)
            .HasConversion(new EntityIdValueConverter<ClassificationCategory>())
            .ValueGeneratedNever();

        builder.ComplexProperty(
            c => c.Name,
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

        builder
            .Property(c => c.ClassificationId)
            .HasConversion(new EntityIdValueConverter<ProductClassification>());

        builder.HasKey(c => c.Id);
    }
}
