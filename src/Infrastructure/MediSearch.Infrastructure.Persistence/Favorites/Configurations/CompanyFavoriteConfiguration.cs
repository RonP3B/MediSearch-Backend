using MediSearch.Core.Domain.Companies;
using MediSearch.Core.Domain.Favorites.CompanyFavorites;
using MediSearch.Core.Domain.SharedKernel.Enums;

namespace MediSearch.Infrastructure.Persistence.Favorites.Configurations;

internal sealed class CompanyFavoriteConfiguration : IEntityTypeConfiguration<CompanyFavorite>
{
    public void Configure(EntityTypeBuilder<CompanyFavorite> builder)
    {
        builder.ToTable("company_favorites");

        builder
            .Property(cf => cf.Id)
            .HasConversion(new EntityIdValueConverter<CompanyFavorite>())
            .ValueGeneratedNever();

        builder.Property(cf => cf.CompanyId).HasConversion(new EntityIdValueConverter<Company>());

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
            .HasOne<Company>()
            .WithMany()
            .HasForeignKey(cf => cf.CompanyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
