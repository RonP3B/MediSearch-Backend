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

        builder
            .Property(c => c.Name)
            .HasConversion(new ClassificationNameValueConverter())
            .HasMaxLength(150);

        builder
            .Property(c => c.ClassificationId)
            .HasConversion(new EntityIdValueConverter<ProductClassification>());

        builder.HasKey(c => c.Id);
        builder.HasIndex(c => new { c.ClassificationId, c.Name }).IsUnique();
    }
}
