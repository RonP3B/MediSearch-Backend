using MediSearch.Core.Application.Companies.DTOs;

namespace MediSearch.Infrastructure.Persistence.Companies.Queries;

internal sealed class Mappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CompanyDto, CompanyDetailsDto>().Ignore(dest => dest.Products);
    }
}
