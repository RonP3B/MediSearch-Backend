using MediSearch.Core.Domain.Companies;

namespace MediSearch.Infrastructure.Persistence.Companies.Configurations;

internal sealed class CompanyTypeConfiguration : IEntityTypeConfiguration<CompanyType>
{
    public void Configure(EntityTypeBuilder<CompanyType> builder)
    {
        builder.ToTable("company_types");

        builder.HasKey(companyType => companyType.Id);

        builder.Property(companyType => companyType.Name).HasMaxLength(50);

        builder.HasData(CompanyType.Pharmacy, CompanyType.Laboratory);
    }
}
