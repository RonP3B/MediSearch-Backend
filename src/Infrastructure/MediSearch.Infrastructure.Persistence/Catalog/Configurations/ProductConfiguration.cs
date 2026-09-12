using MediSearch.Core.Domain.Catalog.ProductClassifications;
using MediSearch.Core.Domain.Catalog.Products;
using MediSearch.Core.Domain.Companies;
using MediSearch.Infrastructure.Persistence.Catalog.ValueConvertors;

namespace MediSearch.Infrastructure.Persistence.Catalog.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder
            .Property(p => p.Id)
            .HasConversion(new EntityIdValueConverter<Product>())
            .ValueGeneratedNever();

        builder.Property(p => p.CompanyId).HasConversion(new EntityIdValueConverter<Company>());

        builder
            .Property(p => p.ClassificationId)
            .HasConversion(new EntityIdValueConverter<ProductClassification>());

        builder.ComplexProperty(
            p => p.Name,
            name =>
            {
                name.Property(p => p.Value)
                    .HasColumnName("name")
                    .HasMaxLength(100);

                name.Property(p => p.Normalized)
                    .HasColumnName("normalized_name")
                    .HasMaxLength(100);
            }
        );

        builder
            .Property(p => p.Description)
            .HasConversion(new CleanTextValueConverter())
            .HasMaxLength(300);

        builder.Property(p => p.Quantity).HasConversion(new QuantityValueConverter());

        builder.ComplexProperty(
            p => p.Price,
            price =>
            {
                price.Property(x => x.Amount).HasColumnName("price_amount");
                price.Property(x => x.Currency).HasColumnName("price_currency").HasMaxLength(3);
            }
        );

        builder.OwnsMany(
            p => p.ImageKeys,
            j =>
            {
                j.ToTable("product_images");
                j.WithOwner().HasForeignKey("product_id");
                j.Property<int>("id").ValueGeneratedOnAdd();
                j.HasKey("id");
                j.Property(i => i.Key).HasColumnName("image_key").HasMaxLength(500);
                j.Property<int>("ordinal").HasColumnName("ordinal");
                j.HasIndex("product_id", "ordinal");
            }
        );

        builder.OwnsMany(
            p => p.CategoryIds,
            j =>
            {
                j.ToTable("products_classification_categories");

                j.WithOwner().HasForeignKey("product_id");

                j.Property(c => c.Value).HasColumnName("category_id");

                j.HasKey("product_id", "Value");

                j.HasIndex("Value");

                // The FK to classification_categories is added in a follow-up migration.
            }
        );

        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.CompanyId);
        builder.HasIndex(p => p.ClassificationId);
        builder.HasIndex(p => p.CreatedAt);

        builder
            .HasOne<Company>()
            .WithMany()
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<ProductClassification>()
            .WithMany()
            .HasForeignKey(p => p.ClassificationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
