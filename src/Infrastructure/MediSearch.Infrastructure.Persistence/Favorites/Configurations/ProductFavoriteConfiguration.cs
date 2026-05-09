using MediSearch.Core.Domain.Catalog.Products;
using MediSearch.Core.Domain.Favorites.ProductFavorites;
using MediSearch.Core.Domain.SharedKernel.Enums;

namespace MediSearch.Infrastructure.Persistence.Favorites.Configurations;

internal sealed class ProductFavoriteConfiguration : IEntityTypeConfiguration<ProductFavorite>
{
    public void Configure(EntityTypeBuilder<ProductFavorite> builder)
    {
        builder.ToTable("product_favorites");

        builder
            .Property(cf => cf.Id)
            .HasConversion(new EntityIdValueConverter<ProductFavorite>())
            .ValueGeneratedNever();

        builder.Property(cf => cf.ProductId).HasConversion(new EntityIdValueConverter<Product>());

        builder.OwnsOne(
            cf => cf.FavoriterAgent,
            b =>
            {
                b.Property(a => a.AgentId).HasColumnName("favoriter_id");
                b.Property(a => a.AgentTypeId).HasColumnName("favoriter_type_id");
                b.HasOne<AgentType>()
                    .WithMany()
                    .HasForeignKey(a => a.AgentTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        );

        builder.HasKey(cf => cf.Id);

        builder
            .HasOne<Product>()
            .WithMany()
            .HasForeignKey(cf => cf.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
