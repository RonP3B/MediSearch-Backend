using MediSearch.Core.Application.Accounts.DTOs;
using MediSearch.Core.Application.Companies.DTOs;
using MediSearch.Core.Domain.Companies;

namespace MediSearch.Core.Application.Companies;

internal sealed class Mappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<Company, CompanyDto>()
            .Map(dest => dest.Province, src => src.Location.Province)
            .Map(dest => dest.Municipality, src => src.Location.Municipality)
            .Map(dest => dest.Address, src => src.Location.Address);

        config
            .NewConfig<RegisterCompanyOwnerDto, RegisterExternalUserDto>()
            .Ignore(dest => dest.IsActive);
    }
}
